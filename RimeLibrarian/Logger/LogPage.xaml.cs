using Microsoft.Win32;
using RimeLibrarian.Logger;
using RimeLibrarian.Tool;
using System.Windows;
using System.Windows.Input;

namespace RimeLibrarian
{
    public partial class LogPage : Window
    {
        public LogPage()
        {
            InitializeComponent();
            ShowLog();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                SaveLog();
        }

        private void ShowLog() => LogBox.Text = Log.ReadAll();

        private static void SaveLog()
        {
            SaveFileDialog sfd = new()
            {
                DefaultExt = ".txt",
                FileName = $"xkjd6.cizu.log({DateTime.Now:yyyy-MM-dd HH-mm-ss}).txt",
                Filter = "词库修改日志 (.txt)|*.txt",
                Title = "日志将放在"
            };
            string location = sfd.ShowDialog() == true
                ? sfd.FileName
                : string.Empty;
            if (location.Length > 0)
                TryCatch.Do("保存日志", () => Log.Save(location));
        }
    }
}
