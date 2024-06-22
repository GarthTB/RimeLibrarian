using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace RimeLibrarian
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += new KeyEventHandler(HotKeys);
        }

        private void HotKeys(object sender, KeyEventArgs e)
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
            if (Log.Any)
            {
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
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            LogPage lp = new();
            lp.ShowDialog();
        }

        #endregion

        #region 加词（左边）

        private HashSet<string> FullCodes = new();
        private List<string> CurrentCodes = new();
        private int length = 4;

        private void RefreshButton()
        {
            ButtonAdd.IsEnabled = PriorityBox.IsEnabled = (WordBox.Text.Length > 1 && CodeCombo.SelectedItem is not null)
                                                          || (WordBox.Text.Length > 0 && CodeBox.Text.Length > 0);
        }

        private void LoadAutoWords()
        {
            CurrentCodes = FullCodes.Select(x => x[..length])
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();
            CodeCombo.ItemsSource = CurrentCodes;
            CodeCombo.SelectedIndex = 0;
        }

        private void UnloadAutoWords()
        {
            CurrentCodes.Clear();
            CodeCombo.ItemsSource = null;
        }

        private void WordBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (WordBox.Text.Length > 1
                && JD.Encode(WordBox.Text, out IEnumerable<string>? codes)
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

        private void CodeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SearchBox.Text = CodeCombo.SelectedItem is null
                ? string.Empty
                : (string)CodeCombo.SelectedItem;
        }

        private void CodeLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CodeCombo.ItemsSource is not null)
            {
                length = (int)CodeLengthSlider.Value;
                LoadAutoWords();
            }
        }

        private void CodeBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
                int priority = GetPriority();
                string code = CodeBox.Text.Length == 0
                    ? (string)CodeCombo.SelectedItem
                    : CodeBox.Text;
                Dict.Add(WordBox.Text, code, priority);
                Log.Add($"添加\t{WordBox.Text}\t{code}\t{priority}");
                WordBox.Text = string.Empty;
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region 其他功能（右边）

        private void LoadResults()
        {
            ItemList.ItemsSource = SearchBox.Text.Length > 0
                ? Dict.AllStartsWith(SearchBox.Text).OrderBy(x => x.Code)
                : null;
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadResults();
        }

        private void ItemList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ItemList.SelectedItem is null)
            {
                ButtonDel.IsEnabled = false;
                ButtonMod.IsEnabled = false;
                ButtonTop.IsEnabled = false;
            }
            else
            {
                ButtonDel.IsEnabled = true;
                ButtonMod.IsEnabled = true;
                ButtonTop.IsEnabled = ItemList.Items.Count > 1 && ItemList.SelectedIndex != 0;
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (Entry)ItemList.SelectedItem;
                Dict.Remove(item.Word, item.Code);
                Log.Add($"删除\t{item.Word}\t{item.Code}\t{item.Priority}");
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonTop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var shortItem = (Entry)ItemList.Items[0];
                var longItem = (Entry)ItemList.SelectedItem;
                Dict.Remove(shortItem.Word, shortItem.Code);
                Dict.Remove(longItem.Word, longItem.Code);
                Dict.Add(longItem.Word, shortItem.Code, longItem.Priority);
                string lengthenCode = JD.Lengthen(shortItem.Word, shortItem.Code);
                Dict.Add(shortItem.Word, lengthenCode, shortItem.Priority);
                Log.Add($"置顶-原短\t{shortItem.Word}\t{shortItem.Code}\t{shortItem.Priority}");
                Log.Add($"置顶-加长\t{shortItem.Word}\t{lengthenCode}\t{shortItem.Priority}");
                Log.Add($"置顶-原长\t{longItem.Word}\t{longItem.Code}\t{longItem.Priority}");
                LoadResults();
            }
            catch (Exception ex)
            {
                MessageBox.Show("置顶失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonMod_Click(object sender, RoutedEventArgs e)
        {
            var item = (Entry)ItemList.SelectedItem;
            ModPage mp = new(item.Word, item.Code, item.Priority);
            try
            {
                mp.ShowDialog();
                if (mp.Modified)
                {
                    Dict.Remove(item.Word, item.Code);
                    Dict.Add(mp.Word, mp.Code, mp.Priority);
                    Log.Add($"修改-原有\t{item.Word}\t{item.Code}\t{item.Priority}");
                    Log.Add($"修改-改为\t{mp.Word}\t{mp.Code}\t{mp.Priority}");
                    LoadResults();
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (mp.Modified)
                {
                    Dict.Remove(item.Word, item.Code);
                    Dict.Add(mp.Word, mp.Code, mp.Priority);
                    Log.Add($"修改-原有\t{item.Word}\t{item.Code}\t{item.Priority}");
                    Log.Add($"修改-改为\t{mp.Word}\t{mp.Code}\t{mp.Priority}");
                    LoadResults();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
