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

        public static bool Contains(string dan)
        {
            return _dict.Any(e => e.Word == dan);
        }

        public static bool Contains(char dan)
        {
            return _dict.Any(e => e.Word == dan.ToString());
        }

        public static IEnumerable<string> FullCodesOf(string dan)
        {
            var codes = _dict.Where(e => e.Word == dan && e.Code.Length > 3)
                             .Select(e => e.Code);
            return codes.Any() ? codes : throw new Exception("找不到符合条件的单字！");
        }

        public static IEnumerable<string> FullCodesOf(char dan)
        {
            var codes = _dict.Where(e => e.Word == dan.ToString() && e.Code.Length > 3)
                             .Select(e => e.Code);
            return codes.Any() ? codes : throw new Exception("找不到符合条件的单字！");
        }
    }
}
