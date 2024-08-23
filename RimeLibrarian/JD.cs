namespace RimeLibrarian
{
    internal class JD
    {
        public static bool Encode(string word, out IEnumerable<char[]>? codes)
        {
            if (IsValid(word, out string validChars))
            {
                codes = CodesOf(validChars);
                return true;
            }
            codes = null;
            return false;
        }

        private static bool IsValid(string originWord, out string validChars)
        {
            var vc = originWord.Where(c => Dan.Contains(c)).ToArray();
            if (vc.Length < 2)
            {
                validChars = string.Empty;
                return false;
            }
            validChars = vc.Length > 4
                ? $"{new string(vc[..3])}{vc[^1]}"
                : new string(vc);
            return true;
        }

        private static IEnumerable<char[]> CodesOf(string word)
        {
            var codesOfChar = word.Select(c => Dan.FullCodesOf(c)
                                                  .Select(s => s[..3])
                                                  .Distinct()
                                                  .ToArray())
                                  .ToArray();
            return word.Length switch
            {
                2 => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     select new[] { c1[0], c1[1], c2[0], c2[1], c1[2], c2[2] },

                3 => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     from c3 in codesOfChar[2]//第三个字的所有编码
                     select new[] { c1[0], c2[0], c3[0], c1[2], c2[2], c3[2] },

                _ => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     from c3 in codesOfChar[2]//第三个字的所有编码
                     from c4 in codesOfChar[3]//第四个字的所有编码
                     select new[] { c1[0], c2[0], c3[0], c4[0], c1[2], c2[2] }
            };
        }

        public static string Lengthen(string word, string prefix)
        {
            if (!Encode(word, out IEnumerable<char[]>? codes) || codes is null)
                throw new ArgumentException("无法为短码的词自动编码！");

            var _codes = codes.Select(c => new string(c))
                              .Where(s => s.StartsWith(prefix))
                              .Distinct();

            if (_codes.Count() != 1)
                throw new Exception("加长方式不唯一！");

            var result = _codes.First();

            for (int i = prefix.Length + 1; i < 6; i++)
                if (!Dict.HasCode(result[..i]))
                    return result[..i];

            return result;
        }
    }
}
