using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace RimeLibrarian
{
    public partial class LogPage : Window
    {
        public LogPage()
        {
            InitializeComponent();
            KeyUp += new KeyEventHandler(HotKeys);
            ShowLog();
        }

        private void HotKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                SaveLog();
        }

        private void ShowLog()
        {
            foreach (string line in Log.ReadAll())
                LogBox.AppendText(line + "\n");
        }

        private void SaveLog()
        {
            if (LogBox.Text.Length == 0)
                MessageBox.Show("没有记录到日志。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string location = SetLocation();
                if (location.Length > 0)
                    try
                    {
                        Log.Save(location);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
        }

        private static string SetLocation()
        {
            SaveFileDialog sfd = new()
            {
                DefaultExt = ".txt",
                FileName = $"xkjd6.cizu.log({DateTime.Now:yyyy-MM-dd-HH-mm-ss}).txt",
                Filter = "词库修改日志 (.txt)|*.txt",
                Title = "日志将放在"
            };
            return sfd.ShowDialog() == true ? sfd.FileName : string.Empty;
        }
    }
}
