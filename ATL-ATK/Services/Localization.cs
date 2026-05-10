using System;
using System.Collections.Generic;

namespace ATL_ATK.Services
{
    public class Localization
    {
        private static Localization _instance;
        public static Localization Instance => _instance ?? (_instance = new Localization());

        public enum Language { Vietnamese, English }
        public Language CurrentLanguage { get; set; } = Language.Vietnamese;

        private Dictionary<string, string> _vietnameseDict;
        private Dictionary<string, string> _englishDict;

        private Localization()
        {
            _vietnameseDict = new Dictionary<string, string>
            {
                { "Error", "Lỗi" },
                { "NoBlockSelected", "Không chọn được Block nào." },
                { "Success", "Thành công" },
                { "TableCreated", "Đã tạo bảng thống kê" }
            };

            _englishDict = new Dictionary<string, string>
            {
                { "Error", "Error" },
                { "NoBlockSelected", "No blocks selected." },
                { "Success", "Success" },
                { "TableCreated", "Table created" }
            };
        }

        public string GetString(string key)
        {
            var dict = CurrentLanguage == Language.Vietnamese ? _vietnameseDict : _englishDict;
            if (dict.TryGetValue(key, out string value))
                return value;
            return key;
        }
    }
}
