using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RimeLibrarian
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Help.Show();
        }

        #region 载入、关闭和日志

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string location = Path.GetFullPath(@"..\xkjd6.cizu.dict.yaml");
            if (LoadLib(location))
            {
                MessageBox.Show($"已自动载入程序上级目录中的词库，如需修改请手动重载。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            location = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime\xkjd6.cizu.dict.yaml";
            if (LoadLib(location))
            {
                MessageBox.Show($"已自动载入Rime默认目录中的词库，如需修改请手动重载。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBox.Show("未能自动载入Rime键道词库。\n请手动选择词库。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            while (Dict.Count == 0)
            {
                location = SetLibLocation();
                if (location.Length > 0)
                    LoadLib(location);
            }
            MessageBox.Show($"已载入指定位置的词库，如需修改请手动重载。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static bool LoadLib(string wordLocation)
        {
            string danLocation = wordLocation.Replace("cizu", "danzi");
            if (File.Exists(wordLocation) && File.Exists(danLocation))
            {
                try
                {
                    Dict.Load(wordLocation);
                    Dan.Load(danLocation);
                    return true;
                }
                catch
                {
                    MessageBox.Show("载入词库失败，请检查词库文件。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }

        private static string SetLibLocation()
        {
            OpenFileDialog ofd = new()
            {
                DefaultExt = ".yaml",
                InitialDirectory = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime",
                Filter = "Rime键道词库 (.yaml)|*.yaml",
                Title = "请选取xkjd6.cizu.dict.yaml文件"
            };
            return ofd.ShowDialog() == true ? ofd.FileName : string.Empty;
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            string location = SetLibLocation();
            if (location.Length > 0)
            {
                LoadLib(location);
                WordBox.Text = string.Empty;
                CodeBox.Text = string.Empty;
                PriorityBox.Text = string.Empty;
                SearchBox.Text = string.Empty;
                Log.Clear();
            }
            else MessageBox.Show("未载入词库，继续使用原有词库。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Log.Any) return;
            try { Dict.Save(); }
            catch (Exception ex)
            {
                MessageBox.Show("保存新的词库失败，错误信息：" + ex.Message + "请另外选个地方存一下！", "糟了！", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveFileDialog sfd = new()
                {
                    DefaultExt = ".txt",
                    FileName = $"xkjd6.cizu.dict({DateTime.Now:yyyy-MM-dd-HH-mm-ss}).txt",
                    Filter = "无法存储的词库 (.txt)|*.txt",
                    Title = "词库将放在"
                };
                while (sfd.ShowDialog() != true)
                    continue;
                Dict.Save(sfd.FileName);
            }
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            LogPage lp = new();
            lp.ShowDialog();
        }

        #endregion

        #region 加词（左边）

        private HashSet<char[]> FullCodes = new();
        private int codeLength = 4;

        private void RefreshButton()
        {
            ButtonAdd.IsEnabled = PriorityBox.IsEnabled = (WordBox.Text.Length > 1 && CodeCombo.SelectedItem is not null)
                                                          || (WordBox.Text.Length > 0 && CodeBox.Text.Length > 0);
        }

        private void SelectItem(string preText, IEnumerable<string> items)
        {
            if (!string.IsNullOrEmpty(preText))
            {
                var itemToChoose = items.FirstOrDefault(x => x.StartsWith(preText) || preText.StartsWith(x));
                CodeCombo.Text = itemToChoose ?? items.FirstOrDefault();
            }
            else CodeCombo.SelectedIndex = 0;
        }

        private void LoadAutoWords()
        {
            var preText = string.IsNullOrWhiteSpace(CodeCombo.Text)
                ? string.Empty
                : CodeCombo.Text;
            var presentCodes = FullCodes.Select(x => new string(x[..codeLength]))
                                        .Distinct()
                                        .OrderBy(x => x);
            CodeCombo.ItemsSource = presentCodes;
            CodeCombo.Foreground = CodeCombo.Items.Count > 1
                ? Brushes.IndianRed
                : Brushes.Black;
            SelectItem(preText, presentCodes);
        }

        private void UnloadAutoWords() => CodeCombo.ItemsSource = null;

        private void WordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WordBox.Text.Length > 1
                && JD.Encode(WordBox.Text, out IEnumerable<char[]>? codes)
                && codes is not null)
            {
                CodeBox.Text = string.Empty;
                FullCodes = codes.ToHashSet();
                LoadAutoWords();
            }
            else
            {
                FullCodes.Clear();
                UnloadAutoWords();
            }
            RefreshButton();
        }

        private void CodeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchBox.Text = CodeCombo.SelectedItem is null
                ? string.Empty
                : (string)CodeCombo.SelectedItem;
        }

        private void CodeLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            codeLength = (int)CodeLengthSlider.Value;
            if (CodeCombo.ItemsSource is not null)
                LoadAutoWords();
        }

        private void CodeLengthSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is Slider slider)
                if (e.Delta > 0) slider.Value++;
                else slider.Value--;
        }

        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBox.Text = CodeBox.Text;
            if (CodeBox.Text.Length > 0)
            {
                if (CodeCombo.ItemsSource is not null)
                    UnloadAutoWords();
            }
            else if (FullCodes.Count > 0)
                LoadAutoWords();
            RefreshButton();
        }

        private int GetPriority()
        {
            if (PriorityBox.Text.Length == 0) return 0;
            if (int.TryParse(PriorityBox.Text, out int priority)) return priority;
            else MessageBox.Show("优先级无法识别，已忽略！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return 0;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = CodeBox.Text.Length == 0
                    ? (string)CodeCombo.SelectedItem
                    : CodeBox.Text;
                Entry newItem = new(WordBox.Text, code, GetPriority());
                Dict.Add(newItem);
                Log.Add("添加", newItem);
                WordBox.Text = string.Empty;//这里会把搜索框清空
                SearchBox.Text = code;//这里会重新填入，相当于刷新
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 其他功能（右边）

        private HashSet<Entry> OriginItems = new();
        private List<Entry> PresentItems = new();

        private void LoadResults()
        {
            if (SearchBox.Text.Length > 0)
            {
                OriginItems = Dict.PrefixIs(SearchBox.Text)
                                  .ToHashSet();
                if (OriginItems.Count > 0)
                {
                    PresentItems = OriginItems.Select(e => e.Clone())
                                              .OrderBy(x => x.Code)
                                              .ToList();
                    ItemList.ItemsSource = PresentItems;
                    ButtonMod.IsEnabled = true;
                    return;
                }
            }
            OriginItems.Clear();
            PresentItems.Clear();
            ItemList.ItemsSource = null;
            ButtonMod.IsEnabled = false;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadResults();
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemList.SelectedItems.Count == 0)
            {
                ButtonDel.IsEnabled = false;
                ButtonCut.IsEnabled = false;
            }
            else if (ItemList.SelectedItems.Count == 1)
            {
                ButtonDel.IsEnabled = true;
                ButtonCut.IsEnabled = ItemList.Items.Count > 1
                                      && ((Entry)ItemList.SelectedItem).Code != SearchBox.Text;
            }
            else
            {
                ButtonDel.IsEnabled = true;
                ButtonCut.IsEnabled = false;
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Entry item in ItemList.SelectedItems)
                {
                    Dict.Remove(item);
                    Log.Add("删除", item);
                }
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var shortItem = PresentItems.Where(x => x.Code == SearchBox.Text).First();
                var longItem = ((Entry)ItemList.SelectedItem).Clone();
                Entry newShort = new(longItem.Word, shortItem.Code, longItem.Priority);
                Entry newLong = new(shortItem.Word, JD.Lengthen(shortItem.Word, shortItem.Code), shortItem.Priority);
                if (newLong.Code.StartsWith(longItem.Code))
                    newLong.Code = longItem.Code;//正好可以互换码位
                Dict.Remove(shortItem);
                Dict.Remove(longItem);
                Dict.Add(newShort);
                Dict.Add(newLong);
                Log.Add("截短-原短", shortItem);
                Log.Add("截短-改为", newShort);
                Log.Add("截短-原长", longItem);
                Log.Add("截短-改为", newLong);
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("截短失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonMod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newItems = PresentItems.Where(item => !OriginItems.Any(x => x.Equals(item)))
                                           .Select(item => item.Clone())
                                           .ToList();

                if (newItems.Count == 0)
                    throw new Exception("没有任何修改！");

                var discards = OriginItems.Where(item => !PresentItems.Any(x => x.Equals(item)))
                                          .Select(item => item.Clone())
                                          .ToList();

                if (newItems.Count != discards.Count)
                    throw new Exception("修改前后数量不一致！");

                foreach (Entry item in discards)
                {
                    Dict.Remove(item);
                    Log.Add("修改-原有", item);
                }

                foreach (Entry item in newItems)
                {
                    Dict.Add(item);
                    Log.Add("修改-改为", item);
                }

                MessageBox.Show($"修改成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
