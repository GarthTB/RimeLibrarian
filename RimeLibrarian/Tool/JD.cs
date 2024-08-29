using System.Collections.Concurrent;

namespace RimeLibrarian.Tool
{
    internal class JD
    {
        public static bool Encode(string word, out char[][] codes)
        {
            codes = GetKeyChars(word, out string keyChars)
                ? CodesOf(keyChars)
                : Array.Empty<char[]>();
            return codes.Length > 0;
        }

        private static bool GetKeyChars(string originWord, out string keyChars)
        {
            var vc = originWord.Where(c => Dan.Contains(c))
                               .ToArray();
            keyChars = vc.Length < 2
                ? string.Empty
                : vc.Length > 4
                    ? $"{new string(vc[..3])}{vc[^1]}"
                    : new string(vc);
            return keyChars.Length > 0;
        }

        private static char[][] CodesOf(string word)
        {
            var codesOfChar = word.Select(c => Dan.KeyCodesOf(c).ToArray())
                                  .ToArray();

            var codes = new ConcurrentBag<char[]>();
            var tasks = new List<Task>();

            switch (word.Length)
            {
                case 2:
                    tasks.AddRange(codesOfChar[0].SelectMany(
                             c1 => codesOfChar[1].Select(
                             c2 => Task.Run(()
                                => codes.Add(new[] { c1[0], c1[1], c2[0], c2[1], c1[2], c2[2] })))));
                    break;

                case 3:
                    tasks.AddRange(codesOfChar[0].SelectMany(
                             c1 => codesOfChar[1].SelectMany(
                             c2 => codesOfChar[2].Select(
                             c3 => Task.Run(()
                                => codes.Add(new[] { c1[0], c2[0], c3[0], c1[2], c2[2], c3[2] }))))));
                    break;

                default:
                    tasks.AddRange(codesOfChar[0].SelectMany(
                             c1 => codesOfChar[1].SelectMany(
                             c2 => codesOfChar[2].SelectMany(
                             c3 => codesOfChar[3].Select(
                             c4 => Task.Run(()
                                => codes.Add(new[] { c1[0], c2[0], c3[0], c4[0], c1[2], c2[2] })))))));
                    break;
            }

            Task.WhenAll(tasks).Wait();

            return codes.ToArray();
        }

        public static string Lengthen(string word, string prefix)
        {
            if (!Encode(word, out char[][] codes))
                throw new ArgumentException("无法为短码的词自动编码！");

            var _codes = codes.Select(c => new string(c))
                              .Where(s => s.StartsWith(prefix))
                              .Distinct()
                              .ToArray();

            if (_codes.Length != 1)
                throw new Exception("加长方式不唯一，或找不到其中某个单字的全码！");

            var result = _codes[0];

            for (int i = prefix.Length + 1; i < 6; i++)
                if (!Dict.HasCode(result[..i]))
                    return result[..i];

            return result;
        }
    }
}
