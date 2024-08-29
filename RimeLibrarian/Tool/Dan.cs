using System.IO;
using System.Text;

namespace RimeLibrarian.Tool
{
    internal static class Dan
    {
        private static readonly HashSet<Entry> _dict = new(0);

        public static void Load(string path)
        {
            _dict.Clear();
            using StreamReader sr = new(path, Encoding.UTF8);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length == 2)
                    _ = _dict.Add(new Entry(parts[0], parts[1]));
                else if (parts.Length == 3)
                    _ = _dict.Add(new Entry(parts[0], parts[1], parts[2]));
            }
            if (_dict.Count == 0)
                throw new Exception("单字文件为空！");
        }

        public static bool Contains(char dan)
        {
            var value = dan.ToString();
            return _dict.Any(e => e.Word == dan.ToString());
        }

        /// <summary>
        /// 某个字的前3码
        /// </summary>
        public static IEnumerable<string> KeyCodesOf(char dan)
        {
            var codes = _dict.Where(e => e.Word == dan.ToString()
                                         && e.Code.Length > 3)
                             .Select(e => e.Code[..3])
                             .Distinct();
            return codes.Any()
                ? codes
                : throw new Exception($"单字中找不到“{dan}”字！");
        }
    }
}
