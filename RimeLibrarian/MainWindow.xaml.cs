using RimeLibrarian.Logger;
using RimeLibrarian.Tool;
using System.ComponentModel;
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
            => Files.AutoLoad();

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            if (Files.LoadLib())
            {
                WordBox.Clear();
                CodeBox.Clear();
                PriorityBox.Clear();
                SearchBox.Clear();
                Log.Clear();
            }
            else MsgB.OkWarn("未载入词库，继续使用原有词库。", "提示");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Log.IsEmpty)
                TryCatch.Do("保存修改后的词库失败，请选择一个位置另存。",
                            () => Dict.Save(),
                            () => Files.SaveAnother());
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
            => _ = new LogPage().ShowDialog();

        #endregion

        #region 加词（左边）

        private HashSet<char[]> FullCodes = new();
        private int codeLength = 4;

        private void RefreshButton()
        {
            ButtonAdd.IsEnabled = PriorityBox.IsEnabled =
                (WordBox.Text.Length > 1 && CodeCombo.SelectedItem is not null)
                || (WordBox.Text.Length > 0 && CodeBox.Text.Length > 0);
        }

        private void SelectItem(string preText, IEnumerable<string> items)
        {
            if (!string.IsNullOrEmpty(preText))
            {
                var itemToChoose = items.FirstOrDefault(
                    x => x.StartsWith(preText) || preText.StartsWith(x));
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
                && JD.Encode(WordBox.Text, out char[][] codes))
            {
                CodeBox.Clear();
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
            if (int.TryParse(PriorityBox.Text, out int priority)
                && priority > 0)
                return priority;
            else MsgB.OkWarn("优先级无法识别，已忽略！", "提示");
            return 0;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
            => TryCatch.Do("添加词条", () =>
            {
                string code = CodeBox.Text.Length == 0
                    ? (string)CodeCombo.SelectedItem
                    : CodeBox.Text;
                Entry newItem = new(WordBox.Text, code, GetPriority());
                Dict.Add(newItem);
                WordBox.Clear(); // 这里会把搜索框清空
                SearchBox.Text = code; // 这里会重新填入，相当于刷新
            });

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
            => LoadResults();

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
            => TryCatch.Do("删除词条", ()
                => Dict.RemoveAll(ItemList.SelectedItems.Cast<Entry>()));

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
            => TryCatch.Do("截短长词", () =>
            {
                var shortItem = PresentItems.Where(x => x.Code == SearchBox.Text).First();
                var longItem = ((Entry)ItemList.SelectedItem).Clone();
                Entry newShort = new(longItem.Word, shortItem.Code, longItem.Priority);
                Entry newLong = new(shortItem.Word, JD.Lengthen(shortItem.Word, shortItem.Code), shortItem.Priority);
                if (newLong.Code.StartsWith(longItem.Code))
                    newLong.Code = longItem.Code; // 正好可以互换码位
                Dict.Remove(shortItem);
                Dict.Remove(longItem);
                Dict.Add(newShort);
                Dict.Add(newLong);
                LoadResults();
            });

        private void ButtonMod_Click(object sender, RoutedEventArgs e)
            => TryCatch.Do("应用修改", () =>
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
                    throw new Exception("修改前后数量不一致！这是不该出现的错误，如果出现请联系开发者。");

                Dict.RemoveAll(discards);
                Dict.AddAll(newItems);

                MsgB.OkInfo($"修改成功！", "提示");
                LoadResults();
            });

        #endregion
    }
}
