using System.Windows;

namespace RimeLibrarian
{
    public partial class ModPage : Window
    {
        public string Word { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Priority { get; set; } = 0;

        public bool Modified { get; set; } = false;

        public ModPage(string word, string code, int priority)
        {
            InitializeComponent();
            WordBox.Text = word;
            CodeBox.Text = code;
            PriorityBox.Text = priority.ToString();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Modified = false;
            Close();
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            Modified = true;
            Word = WordBox.Text;
            Code = CodeBox.Text;
            Priority = int.TryParse(PriorityBox.Text, out int priority)
                ? priority
                : throw new FormatException("优先级无法识别，已令其为0。");
            Close();
        }
    }
}
