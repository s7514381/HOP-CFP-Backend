// Mime Sniffing
// 透過偵測檔案內容來推斷檔案 Mime 類型

// 運作方式解說請見: https://stackoverflow.com/questions/18299806/how-to-check-file-mime-type-with-javascript-before-upload
//     頁面備份: HOP_CFP_Backend\docs\How to check file MIME type with javascript before upload_ - Stack Overflow.html
// 更多檔案簽名請見: https://www.garykessler.net/library/file_sigs.html 
//     頁面備份: HOP_CFP_Backend\docs\File Signatures.html

// 請同時維護前端程式碼 wwwroot\js\mime-sniffer.js

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Utility
{
    /// <summary>
    /// 提供 Mime 嗅探的功能，檢查檔案內容是否與附檔名相符
    /// </summary>
    public static class MimeSniffer
    {
        private static readonly Dictionary<string, string[]> magicNumberDict = new Dictionary<string, string[]>
        {
            { "image/png", new string[] { "89504e47" } },
            { "image/jpeg", new string[]
                {
                    "ffd8ffe0",
                    "ffd8ffe1",
                    "ffd8ffe2",
                    "ffd8ffe3",
                    "ffd8ffe8"
                }
            },
            { "application/pdf", new string[] { "25504446" } },
            { "image/gif", new string[] { "47494638" } },
            {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                new string[] { "504b0304" }
            },
            { "application/vnd.ms-excel", new string[] { "d0cf11e0" } },
        };

        /// <summary>
        /// 檢查資料是否與檔案類型相符
        /// </summary>
        /// <param name="data">資料內容</param>
        /// <param name="contentType">要檢查的檔案類型</param>
        public static bool IsDataMatchContentType(ref byte[] data, string contentType)
        {
            if (!magicNumberDict.ContainsKey(contentType))
                throw new NotImplementedException($"檔案格式檢查目前不支援 {contentType} 格式");
            var header = BitConverter.ToString(data.Take(4).ToArray()).Replace("-", "").ToLower();
            return magicNumberDict[contentType].Contains(header);
        }

        /// <summary>
        ///  檢查上傳的檔案是否與檔案類型相符
        /// </summary>
        /// <param name="file">上傳的檔案</param>
        public static async Task<bool> IsDataMatchContentTypeAsync(this IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var data = new byte[4];
            await stream.ReadAsync(data, 0, 4);
            return IsDataMatchContentType(ref data, file.ContentType);
        }
    }
}
