using System;
using System.Collections.Generic;
using System.Linq;
using ATL_ATK.Models;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service parse và evaluate biểu thức Tag.
    /// Tương đương logic trong hàm C:atk và ND:ATK_str->list, ND:ATK_math trong LISP.
    /// 
    /// Cú pháp biểu thức Tag:
    ///   - Tên Tag: TENBV, MADUAN
    ///   - Literal: "nội dung" (trong dấu nháy kép)
    ///   - Biến đặc biệt: %%STT, %%NAME, %%BLK, %%X, %%Y, %%Z
    ///   - Phép toán: %%+, %%-, %%*, %%/
    ///   - Hằng số: %%2, %%3.5
    ///   - Kết hợp: MADUAN " - " MABV  →  giá trị MADUAN + " - " + giá trị MABV
    ///   - Phép tính: TAG %%+ %%2 %%* %%3.5  →  (TAG+2)*3.5 (trái qua phải, không ưu tiên)
    /// </summary>
    public static class TagParserService
    {
        /// <summary>
        /// Parse chuỗi biểu thức Tag thành danh sách Token.
        /// Tương đương ND:ATK_str->list trong LISP.
        /// </summary>
        public static List<TagToken> ParseExpression(string expression)
        {
            var tokens = new List<TagToken>();

            if (string.IsNullOrWhiteSpace(expression))
                return tokens;

            int i = 0;
            int length = expression.Length;

            while (i < length)
            {
                // Bỏ qua khoảng trắng đầu token
                if (char.IsWhiteSpace(expression[i]) && i + 1 < length && expression[i + 1] != '"' && !IsPercentPercent(expression, i + 1))
                {
                    i++;
                    continue;
                }

                // Literal string: "..."
                if (expression[i] == '"')
                {
                    int endQuote = expression.IndexOf('"', i + 1);
                    if (endQuote > i)
                    {
                        string literal = expression.Substring(i + 1, endQuote - i - 1);
                        tokens.Add(new TagToken(TagTokenType.Literal, literal));
                        i = endQuote + 1;
                    }
                    else
                    {
                        // Không tìm được dấu nháy đóng → coi toàn bộ phần còn lại là literal
                        tokens.Add(new TagToken(TagTokenType.Literal, expression.Substring(i + 1)));
                        break;
                    }
                    continue;
                }

                // Special variables: %%STT, %%NAME, %%BLK, %%X, %%Y, %%Z
                // Math operators: %%+, %%-, %%*, %%/
                // Numeric constants: %%2, %%3.5
                if (IsPercentPercent(expression, i))
                {
                    string remaining = expression.Substring(i + 2).TrimStart();
                    int tokenEnd = FindTokenEnd(remaining);
                    string tokenValue = remaining.Substring(0, tokenEnd).Trim();

                    if (string.Equals(tokenValue, "STT", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%STT"));
                    }
                    else if (string.Equals(tokenValue, "NAME", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%NAME"));
                    }
                    else if (string.Equals(tokenValue, "BLK", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%BLK"));
                    }
                    else if (string.Equals(tokenValue, "X", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%X"));
                    }
                    else if (string.Equals(tokenValue, "Y", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%Y"));
                    }
                    else if (string.Equals(tokenValue, "Z", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new TagToken(TagTokenType.SpecialVariable, "%%Z"));
                    }
                    else if (tokenValue == "+" || tokenValue == "-" || tokenValue == "*" || tokenValue == "/")
                    {
                        tokens.Add(new TagToken(TagTokenType.MathOperator, tokenValue));
                    }
                    else if (double.TryParse(tokenValue, out _))
                    {
                        tokens.Add(new TagToken(TagTokenType.NumericConstant, tokenValue));
                    }

                    i += 2 + (remaining.Length - remaining.TrimStart().Length) + tokenEnd;
                    continue;
                }

                // Attribute Tag name (word)
                if (char.IsLetterOrDigit(expression[i]) || expression[i] == '_')
                {
                    int start = i;
                    while (i < length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                    {
                        i++;
                    }
                    string tagName = expression.Substring(start, i - start);
                    tokens.Add(new TagToken(TagTokenType.AttributeTag, tagName));
                    continue;
                }

                // Bỏ qua ký tự không nhận diện được (space giữa tokens)
                i++;
            }

            return tokens;
        }

        /// <summary>
        /// Evaluate biểu thức Tag với dữ liệu Block cụ thể.
        /// Tương đương logic nội bộ trong C:atk xử lý lst_tag.
        /// </summary>
        /// <param name="tokens">Danh sách token đã parse</param>
        /// <param name="blockData">Dữ liệu Block</param>
        /// <param name="sequenceNumber">Số thứ tự (cho %%STT)</param>
        /// <param name="precision">Độ chính xác số thập phân (LUPREC)</param>
        public static string EvaluateExpression(
            List<TagToken> tokens,
            BlockData blockData,
            int sequenceNumber,
            int precision)
        {
            if (tokens == null || tokens.Count == 0)
                return "";

            // Bước 1: Resolve tất cả tokens thành giá trị
            var resolvedValues = new List<object>(); // string hoặc double hoặc char (toán tử)

            foreach (TagToken token in tokens)
            {
                switch (token.TokenType)
                {
                    case TagTokenType.Literal:
                        resolvedValues.Add(token.Value);
                        break;

                    case TagTokenType.SpecialVariable:
                        resolvedValues.Add(ResolveSpecialVariable(token.Value, blockData, sequenceNumber, precision));
                        break;

                    case TagTokenType.AttributeTag:
                        string attValue;
                        if (blockData.Attributes.TryGetValue(token.Value.ToUpper(), out attValue))
                        {
                            resolvedValues.Add(Helpers.StringHelper.UnformatMText(attValue));
                        }
                        else
                        {
                            resolvedValues.Add(null); // tag không tồn tại
                        }
                        break;

                    case TagTokenType.MathOperator:
                        resolvedValues.Add(token.Value[0]); // '+', '-', '*', '/'
                        break;

                    case TagTokenType.NumericConstant:
                        resolvedValues.Add(double.Parse(token.Value));
                        break;
                }
            }

            // Bước 2: Kiểm tra có phép tính không
            bool hasMath = resolvedValues.Any(v => v is char || v is double);

            if (hasMath)
            {
                return EvaluateMathExpression(resolvedValues, precision);
            }

            // Bước 3: Nối chuỗi đơn giản
            return string.Concat(resolvedValues.Where(v => v != null).Select(v => v.ToString()));
        }

        // -------------------------------------------------------
        //  PRIVATE HELPERS
        // -------------------------------------------------------

        private static bool IsPercentPercent(string expression, int index)
        {
            return index + 1 < expression.Length
                   && expression[index] == '%'
                   && expression[index + 1] == '%';
        }

        private static int FindTokenEnd(string str)
        {
            int i = 0;
            while (i < str.Length && !char.IsWhiteSpace(str[i]) && str[i] != '"')
            {
                i++;
            }
            return Math.Max(i, 1);
        }

        private static object ResolveSpecialVariable(string variable, BlockData blockData, int seqNum, int precision)
        {
            switch (variable.ToUpper())
            {
                case "%%STT":
                    return seqNum.ToString();
                case "%%NAME":
                    return blockData.BlockName ?? "";
                case "%%BLK":
                    return "%%BLK" + (blockData.BlockName ?? "");
                case "%%X":
                    return Math.Round(blockData.X, precision).ToString("F" + precision);
                case "%%Y":
                    return Math.Round(blockData.Y, precision).ToString("F" + precision);
                case "%%Z":
                    return Math.Round(blockData.Z, precision).ToString("F" + precision);
                default:
                    return "";
            }
        }

        /// <summary>
        /// Tính toán biểu thức từ trái sang phải (không ưu tiên * / trước + -).
        /// Tương đương ND:ATK_math trong LISP.
        /// </summary>
        private static string EvaluateMathExpression(List<object> values, int precision)
        {
            // Tách thành: prefix (string đầu), phần tính toán, suffix (string cuối)
            string prefix = "";
            string suffix = "";
            var mathPart = new List<object>();

            // Tìm prefix (các string ở đầu)
            int startIndex = 0;
            while (startIndex < values.Count && values[startIndex] is string)
            {
                prefix += values[startIndex].ToString();
                startIndex++;
            }

            // Tìm suffix (các string ở cuối)
            int endIndex = values.Count - 1;
            var suffixParts = new List<string>();
            while (endIndex >= startIndex && values[endIndex] is string)
            {
                suffixParts.Insert(0, values[endIndex].ToString());
                endIndex--;
            }
            suffix = string.Concat(suffixParts);

            // Phần giữa là math
            for (int i = startIndex; i <= endIndex; i++)
            {
                mathPart.Add(values[i]);
            }

            if (mathPart.Count == 0)
                return prefix + suffix;

            // Chuyển tất cả giá trị thành số, toán tử giữ nguyên
            double result = 0;
            bool hasInitial = false;

            // Tìm giá trị đầu tiên
            int mi = 0;
            if (mi < mathPart.Count)
            {
                object first = mathPart[mi];
                if (first is double d)
                {
                    result = d;
                    hasInitial = true;
                    mi++;
                }
                else if (first is string s && double.TryParse(s, out double parsed))
                {
                    result = parsed;
                    hasInitial = true;
                    mi++;
                }
            }

            // Xử lý tuần tự: operator + operand
            while (mi < mathPart.Count)
            {
                object current = mathPart[mi];

                if (current is char op)
                {
                    mi++;
                    if (mi < mathPart.Count)
                    {
                        double operand = ToDouble(mathPart[mi]);
                        switch (op)
                        {
                            case '+': result += operand; break;
                            case '-': result -= operand; break;
                            case '*': result *= operand; break;
                            case '/':
                                if (operand != 0)
                                    result /= operand;
                                break;
                        }
                        mi++;
                    }
                }
                else
                {
                    if (!hasInitial)
                    {
                        result = ToDouble(current);
                        hasInitial = true;
                    }
                    mi++;
                }
            }

            string resultStr = Helpers.StringHelper.FormatNumber(result, precision);
            return prefix + resultStr + suffix;
        }

        private static double ToDouble(object value)
        {
            if (value is double d)
                return d;
            if (value is string s)
            {
                double.TryParse(s, out double parsed);
                return parsed;
            }
            return 0;
        }
    }
}
