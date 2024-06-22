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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //程序上级目录
            string location = Path.GetFullPath(@"..\xkjd6.cizu.dict.yaml");
            if (LoadLib(location))
            {
                MessageBox.Show($"已自动载入程序上级目录中的词库，如需修改请手动重载。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //Rime默认目录
            location = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Rime\xkjd6.cizu.dict.yaml";
            if (LoadLib(location))
            {
                MessageBox.Show($"已自动载入Rime默认目录中的词库，如需修改请手动重载。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //都没有
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

        private static string SetLibLocation()//选择词库位置
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
                LoadLib(location);
            else MessageBox.Show("未载入词库，继续使用原有词库。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            LogPage lp = new();
            Hide();
            lp.ShowDialog();
            Show();
        }
    }
}
