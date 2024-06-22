using System.IO;

namespace RimeLibrarian
{
    internal static class Dan
    {
        private static readonly HashSet<Entry> _dict = new(0);

        public static void Load(string path)
        {
            _dict.Clear();
            using StreamReader sr = new(path, System.Text.Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2)
                    _dict.Add(new Entry(parts[0], parts[1]));
                else if (parts.Length == 3)
                    _dict.Add(new Entry(parts[0], parts[1], parts[2]));
            }
            if (!_dict.Any())
                throw new Exception("单字文件为空！");
        }

        public static bool Contains<T>(T dan) where T : IComparable
        {
            string? value = dan.ToString();
            return _dict.Any(e => e.Word.CompareTo(value) == 0);
        }

        public static IEnumerable<string> FullCodesOf<T>(T dan) where T : IComparable
        {
            var codes = _dict.Where(e => e.Word.CompareTo(dan.ToString()) == 0 && e.Code.Length > 3)
                             .Select(e => e.Code);
            return codes.Any() ? codes : throw new Exception("找不到符合条件的单字！");
        }
    }
}
