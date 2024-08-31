using RimeLibrarian.Tool;
using System.IO;
using System.Text;

namespace RimeLibrarian.Logger
{
    internal static class Log
    {
        private static readonly List<string> _log = new(0);

        public static bool IsEmpty => _log.Count == 0;

        public static void Add(string message, Entry entry)
        {
            _log.Add(entry.Priority == 0
                ? $"{message}\t{entry.Word}\t{entry.Code}"
                : $"{message}\t{entry.Word}\t{entry.Code}\t{entry.Priority}");
        }

        public static void Clear() => _log.Clear();

        public static string ReadAll()
        {
            var sb = new StringBuilder();
            foreach (var log in _log)
                _ = sb.AppendLine(log);
            return sb.ToString();
        }

        public static void Save(string path)
        {
            if (_log.Count == 0)
                throw new InvalidOperationException("没有记录到日志。");

            if (File.Exists(path))
                MsgB.OkInfo("同名文件已存在，将续写。", "提示");

            using StreamWriter sw = new(path, true, Encoding.UTF8);
            foreach (var log in _log)
                sw.WriteLine(log);

            MsgB.OkInfo("日志已保存。", "提示");
        }
    }
}
