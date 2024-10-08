﻿using System.IO;
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
                {
                    if (!_dict.Add(new Entry(parts[0], parts[1])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]}");
                }
                else if (parts.Length == 3)
                {
                    if (!_dict.Add(new Entry(parts[0], parts[1], parts[2])))
                        throw new Exception($"无法读取单字文件中的：{parts[0]} {parts[1]} {parts[2]}");
                }
            }
            if (_dict.Count == 0)
                throw new Exception("单字文件为空！");
        }

        public static bool Contains(char dan)
            => _dict.Any(e => e.Word == dan.ToString());

        /// <summary>
        /// 某个字的前3码
        /// </summary>
        public static char[][][] GetKeyCodes(string word)
        {
            var result = new char[word.Length][][];
            for (int i = 0; i < word.Length; i++)
            {
                var c = word[i];
                result[i] = _dict.Where(e => e.Word == c.ToString() && e.Code.Length > 3)
                                 .Select(e => e.Code.ToCharArray(0, 3))
                                 .Distinct()
                                 .ToArray();
                if (result[i].Length == 0)
                    throw new Exception($"单字中找不到“{c}”字！");
            }
            return result;
        }
    }
}
