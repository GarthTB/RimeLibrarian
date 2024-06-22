namespace RimeLibrarian
{
    internal class JD
    {
        public static bool Encode(string word, out IEnumerable<string>? codes)
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
            var vc = originWord.Where(c => Dan.Contains(c)).ToArray().AsSpan();
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

        private static IEnumerable<string> CodesOf(string word)
        {
            List<IEnumerable<string>> codesOfChar = word.Select(c => Dan.FullCodesOf(c)).ToList();
            return word.Length switch
            {
                2 => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     select new string(new[] { c1[0], c1[1], c2[0], c2[1], c1[2], c2[2] }),

                3 => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     from c3 in codesOfChar[2]//第三个字的所有编码
                     select new string(new[] { c1[0], c2[0], c3[0], c1[2], c2[2], c3[2] }),

                _ => from c1 in codesOfChar[0]//第一个字的所有编码
                     from c2 in codesOfChar[1]//第二个字的所有编码
                     from c3 in codesOfChar[2]//第三个字的所有编码
                     from c4 in codesOfChar[3]//第四个字的所有编码
                     select new string(new[] { c1[0], c2[0], c3[0], c4[0], c1[2], c2[2] })
            };
        }
    }
}
