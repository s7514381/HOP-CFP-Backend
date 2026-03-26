using Ganss.Xss;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HOP_CFP_Backend.Utility
{
    public class HtmlSanitizerConverter : JsonConverter<string>
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerConverter()
        {
            _sanitizer = new HtmlSanitizer();

            // --- 設定白名單 ---
            // 1. 確保允許 span 標籤
            _sanitizer.AllowedTags.Add("span");

            // 2. 確保允許常用的屬性
            _sanitizer.AllowedAttributes.Add("class");
            _sanitizer.AllowedAttributes.Add("data-id"); // 允許你範例中的 data-id
            _sanitizer.AllowedAttributes.Add("id");

            // 3. (選填) 如果你還有其他屬性如 style, 也可以在這邊加
             _sanitizer.AllowedAttributes.Add("style"); 
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _sanitizer.Sanitize(reader.GetString() ?? "");
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(_sanitizer.Sanitize(value));
        }
    }
}
