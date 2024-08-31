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
            var codesOfChar = Dan.GetKeyCodes(word);
            var codes = new List<char[]>();

            switch (word.Length)
            {
                case 2:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            codes.Add(new[] { c1[0], c1[1], c2[0], c2[1], c1[2], c2[2] });
                    break;

                case 3:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            foreach (var c3 in codesOfChar[2])
                                codes.Add(new[] { c1[0], c2[0], c3[0], c1[2], c2[2], c3[2] });
                    break;

                default:
                    foreach (var c1 in codesOfChar[0])
                        foreach (var c2 in codesOfChar[1])
                            foreach (var c3 in codesOfChar[2])
                                foreach (var c4 in codesOfChar[3])
                                    codes.Add(new[] { c1[0], c2[0], c3[0], c4[0], c1[2], c2[2] });
                    break;
            }

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

            if (_codes.Length == 0)
                throw new Exception("短码和自动编码不匹配！");
            if (_codes.Length > 1)
                throw new Exception("短码加长方式不唯一！");

            var result = _codes[0];

            for (int i = prefix.Length + 1; i < 6; i++)
                if (!Dict.HasCode(result[..i]))
                    return result[..i];

            return result;
        }
    }
}
