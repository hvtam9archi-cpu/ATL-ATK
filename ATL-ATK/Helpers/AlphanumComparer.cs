using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ATL_ATK.Helpers
{
    /// <summary>
    /// So sánh chuỗi theo thứ tự tự nhiên (alphanumeric).
    /// Tương đương hàm ND:alphanumcompare trong LISP.
    /// Ví dụ: "A2" < "A10" (thay vì so sánh ký tự thì "A10" < "A2").
    /// </summary>
    public class AlphanumComparer : IComparer<string>
    {
        private readonly bool _caseSensitive;

        public AlphanumComparer(bool caseSensitive = false)
        {
            _caseSensitive = caseSensitive;
        }

        public int Compare(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            string[] tokensX = Tokenize(x);
            string[] tokensY = Tokenize(y);

            int minLength = Math.Min(tokensX.Length, tokensY.Length);

            for (int i = 0; i < minLength; i++)
            {
                string tokenX = tokensX[i];
                string tokenY = tokensY[i];

                bool isNumX = double.TryParse(tokenX, out double numX);
                bool isNumY = double.TryParse(tokenY, out double numY);

                int result;
                if (isNumX && isNumY)
                {
                    result = numX.CompareTo(numY);
                }
                else
                {
                    StringComparison comparison = _caseSensitive
                        ? StringComparison.Ordinal
                        : StringComparison.OrdinalIgnoreCase;
                    result = string.Compare(tokenX, tokenY, comparison);
                }

                if (result != 0)
                    return result;
            }

            return tokensX.Length.CompareTo(tokensY.Length);
        }

        /// <summary>
        /// Tách chuỗi thành các token số và chữ xen kẽ.
        /// Ví dụ: "ABC123DEF45" => ["ABC", "123", "DEF", "45"]
        /// </summary>
        private static string[] Tokenize(string input)
        {
            var matches = Regex.Matches(input, @"(\d+\.?\d*)|(\D+)");
            var tokens = new List<string>(matches.Count);
            foreach (Match match in matches)
            {
                tokens.Add(match.Value);
            }
            return tokens.ToArray();
        }
    }
}
