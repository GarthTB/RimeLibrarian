namespace RimeLibrarian.Tool
{
    internal class Entry
    {
        // 这里不能用字段，因为字段把显示在ItemList里
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

        public Entry Clone() => new(Word, Code, Priority);

        public bool Equals(Entry other)
            => Word == other.Word
               && Code == other.Code
               && Priority == other.Priority;
    }
}
