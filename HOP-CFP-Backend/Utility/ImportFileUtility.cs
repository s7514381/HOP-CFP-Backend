using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Text;

namespace HOP_CFP_Backend.Utility
{
    public static class ImportFileUtility
    {
        public static async Task<List<string>> ReadImportLinesAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension is ".xlsx" or ".xlsm" or ".xls")
            {
                using var stream = file.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null || worksheet.Dimension == null)
                {
                    return [];
                }

                var rows = new List<string>();
                for (int rowIndex = worksheet.Dimension.Start.Row; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
                {
                    var values = new List<string>();
                    for (int columnIndex = worksheet.Dimension.Start.Column; columnIndex <= worksheet.Dimension.End.Column; columnIndex++)
                    {
                        values.Add(EscapeCsvValue(worksheet.Cells[rowIndex, columnIndex].Text ?? string.Empty));
                    }

                    rows.Add(string.Join(",", values));
                }

                return rows;
            }

            using var csvStream = file.OpenReadStream();
            using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var content = await reader.ReadToEndAsync();

            return content.Replace("\r\n", "\n").Replace('\r', '\n')
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        public static int GetHeaderIndex(List<string> headers, string[] candidates)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                if (candidates.Any(candidate => string.Equals(headers[i], candidate, StringComparison.OrdinalIgnoreCase)))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string GetValue(List<string> values, int index)
        {
            if (index < 0 || index >= values.Count)
            {
                return string.Empty;
            }

            return values[index].Trim();
        }

        public static List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int index = 0; index < line.Length; index++)
            {
                char currentChar = line[index];
                char nextChar = index + 1 < line.Length ? line[index + 1] : '\0';

                if (currentChar == '"')
                {
                    if (inQuotes && nextChar == '"')
                    {
                        current.Append('"');
                        index += 1;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (currentChar == ',' && !inQuotes)
                {
                    values.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(currentChar);
            }

            values.Add(current.ToString());
            return values;
        }

        private static string EscapeCsvValue(string value)
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}