namespace ATL_ATK.Models
{
    /// <summary>
    /// Loại phần tử trong biểu thức Tag đã parse.
    /// </summary>
    public enum TagTokenType
    {
        /// <summary>Chuỗi cố định (literal string trong dấu "")</summary>
        Literal,

        /// <summary>Tên Attribute Tag (biến)</summary>
        AttributeTag,

        /// <summary>Ký hiệu đặc biệt: %%STT, %%NAME, %%BLK, %%X, %%Y, %%Z</summary>
        SpecialVariable,

        /// <summary>Phép toán: %%+, %%-, %%*, %%/</summary>
        MathOperator,

        /// <summary>Số thực hằng số: %%2, %%3.5</summary>
        NumericConstant
    }

    /// <summary>
    /// Một token trong biểu thức Tag.
    /// Ví dụ: "MADUAN \"-\" MABV" => [Att(MADUAN), Lit(" - "), Att(MABV)]
    /// </summary>
    public class TagToken
    {
        public TagTokenType TokenType { get; set; }

        /// <summary>Giá trị: tên tag, chuỗi literal, toán tử, hoặc số</summary>
        public string Value { get; set; }

        public TagToken(TagTokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}
