namespace RimeLibrarian
{
    internal class Entry
    {
        public string Word { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Priority { get; set; } = 0;

        public Entry(string word, string code, string priority)
        {
            Word = word;
            Code = code;
            if (!int.TryParse(priority, out int temp))
                throw new ArgumentException("遇到了无法识别的优先级！");
            Priority = temp;
        }

        public Entry(string word, string code, int priority = 0)
        {
            Word = word;
            Code = code;
            Priority = priority;
        }
    }
}
