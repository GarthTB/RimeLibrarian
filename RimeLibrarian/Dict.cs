﻿using System.IO;

namespace RimeLibrarian
{
    internal static class Dict
    {
        private static readonly List<string> _shit = new(0);
        private static readonly HashSet<Entry> _dict = new(0);
        private static string _path = string.Empty;

        public static int Count => _dict.Count;

        public static void Load(string path)
        {
            _shit.Clear();
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
                else _shit.Add(line);
            }
            if (!_dict.Any())
                throw new Exception("词库文件为空！");
            _path = path;
        }

        public static void Save(string? path = null)
        {
            if (_path.Length == 0) return;
            path ??= _path;
            using StreamWriter sw = new(path, false, System.Text.Encoding.UTF8);
            if (_shit.Any())
                foreach (var shit in _shit)
                    sw.WriteLine(shit);
            var sortedDict = _dict.OrderBy(e => e.Code).ThenByDescending(e => e.Priority);
            foreach (var sd in sortedDict)
                sw.WriteLine(sd.Priority == 0
                    ? $"{sd.Word}\t{sd.Code}"
                    : $"{sd.Word}\t{sd.Code}\t{sd.Priority}");
        }

        public static void Add(Entry entry)
        {
            if (HasEntry(entry))
                throw new Exception("词库中已存在该词条！");
            _dict.Add(entry.Clone());
        }

        public static void Remove(Entry entry)
        {
            if (!HasEntry(entry))
                throw new Exception("词库中不存在该词条！");
            _dict.RemoveWhere(e => e.Equals(entry));
        }

        public static bool HasWord(string word)
        {
            return _dict.Any(e => e.Word == word);
        }

        public static bool HasCode(string code)
        {
            return _dict.Any(e => e.Code == code);
        }

        public static bool HasEntry(Entry entry)
        {
            return _dict.Any(e => e.Equals(entry));
        }

        public static IEnumerable<string> WordsOf(string code)
        {
            var words = _dict.Where(e => e.Code == code).Select(e => e.Word);
            return words.Any() ? words : throw new Exception("词库中不存在该编码！");
        }

        public static IEnumerable<string> CodesOf(string word)
        {
            var codes = _dict.Where(e => e.Word == word).Select(e => e.Code);
            return codes.Any() ? codes : throw new Exception("词库中不存在该词！");
        }

        public static IEnumerable<Entry> PrefixIs(string prefix)
        {
            return _dict.Where(e => e.Code.StartsWith(prefix))
                        .Select(e => e.Clone());
        }
    }
}
