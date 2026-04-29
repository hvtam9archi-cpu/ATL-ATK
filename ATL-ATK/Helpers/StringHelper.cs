using System.Text;
using System.Text.RegularExpressions;

namespace ATL_ATK.Helpers
{
    /// <summary>
    /// Các hàm tiện ích xử lý chuỗi.
    /// Tương đương các hàm ND:str-subst, ND:unformat, ND:strnump, v.v. trong LISP.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Thay thế tất cả chuỗi con trong chuỗi.
        /// Tương đương ND:str-subst trong LISP.
        /// </summary>
        public static string SubstituteString(string source, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue))
                return source;
            return source.Replace(oldValue, newValue);
        }

        /// <summary>
        /// Xóa format code (MText) khỏi chuỗi.
        /// Tương đương ND:unformat trong LISP.
        /// Loại bỏ: \P, \pxi..;, \O, \L, \~, \\, \{, \}, 
        ///          \Ffilename;, \Hheight;, \Hheightx;, \S...;, \T..;,
        ///          \Q..;, \W..;, \A..;, \C..;, \fFontFile..;, {...}, etc.
        /// </summary>
        public static string UnformatMText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string result = input;

            // Xóa overline/underline codes
            result = Regex.Replace(result, @"\\[OoLl]", "");

            // Xóa \P (paragraph break) -> space
            result = result.Replace("\\P", " ");

            // Xóa \~ (non-breaking space) -> space
            result = result.Replace("\\~", " ");

            // Xóa escaped characters
            result = result.Replace("\\\\", "\\");
            result = result.Replace("\\{", "{");
            result = result.Replace("\\}", "}");

            // Xóa font/height/color codes: \Ffilename;  \Hheight;  \Ccolor;
            result = Regex.Replace(result, @"\\[FfHhCcTtQqWwAa][^;]*;", "");

            // Xóa stacking: \S...;
            result = Regex.Replace(result, @"\\S[^;]*;", "");

            // Xóa paragraph formatting: \pxi..;
            result = Regex.Replace(result, @"\\p[^;]*;", "");

            // Xóa cặp ngoặc nhọn
            result = result.Replace("{", "").Replace("}", "");

            return result.Trim();
        }

        /// <summary>
        /// Kiểm tra chuỗi có phải là số thực không.
        /// Tương đương ND:strnump trong LISP.
        /// </summary>
        public static bool IsNumericString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            return double.TryParse(str, out _);
        }

        /// <summary>
        /// Định dạng số: loại bỏ trailing zeros.
        /// Ví dụ: "3.500000" -> "3.5", "4.000000" -> "4"
        /// Tương đương (vl-string-right-trim "." (vl-string-right-trim "0" ...)) trong LISP.
        /// </summary>
        public static string FormatNumber(double value, int precision = 8)
        {
            string formatted = value.ToString("F" + precision);
            formatted = formatted.TrimEnd('0').TrimEnd('.');
            if (string.IsNullOrEmpty(formatted))
                formatted = "0";
            return formatted;
        }

        /// <summary>
        /// Tách chuỗi thành danh sách theo delimiter.
        /// Tương đương ND:str->list trong LISP.
        /// </summary>
        public static string[] SplitString(string input, string delimiter)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];
            return input.Split(new[] { delimiter }, System.StringSplitOptions.None);
        }
    }
}
