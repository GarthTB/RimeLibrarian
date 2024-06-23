using System.IO;

namespace RimeLibrarian
{
    internal static class Log
    {
        private static readonly List<string> _log = new(0);

        public static bool Any => _log.Any();

        public static void Add(string message) => _log.Add(message);

        public static void Add(string message, Entry entry) => _log.Add($"{message}\t{entry.Word}\t{entry.Code}\t{entry.Priority}");

        public static void Clear() => _log.Clear();

        public static IEnumerable<string> ReadAll() => _log;

        public static void Save(string path)
        {
            using StreamWriter sw = new(path, false, System.Text.Encoding.UTF8);
            if (!_log.Any()) throw new InvalidOperationException("没有记录到日志。");
            foreach (var log in _log)
                sw.WriteLine(log);
        }
    }
}
