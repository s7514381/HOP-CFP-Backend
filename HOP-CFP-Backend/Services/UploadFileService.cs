using Microsoft.AspNetCore.Http;
using HOP_CFP_Backend.Library.Models.UploadFile;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HOP_CFP_Backend.Services
{
    public class UploadFileService : BaseService
    {
        public UploadFileService(BaseServiceArgument argument) : base(argument) { }

        /// <summary>
        /// 將檔案儲存並將回寫到相應欄位(只儲存路徑的字串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task UploadFileAndSyncModel<T>(T model, IFormFileCollection files, string category) where T : new()
        {
            string keyValue = model.GetFieldValue(CommonUtility.GetKeyAttribute<T>())?.ToString();

            foreach (IFormFile file in files)
            {
                if (file.Length == 0) { continue; }

                (string folderPath, string savePath) = GetFileInfo(category, file.FileName, keyValue);

                using var stream = new FileStream($"{folderPath}/{file.FileName}", FileMode.Create);
                file.CopyTo(stream);

                List<string> splited = file.Name.Replace("]", "").Split('[').ToList();
                model = await SyncFormValueToModel(model, splited, savePath);
            }
        }

        /// <summary>
        /// 只把檔案同步到欄位裡，沒有真的上傳檔案，用來檢查是否有上傳檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task FilesSyncModel<T>(T model, IFormFileCollection files, string category) where T : new()
        {
            string keyValue = model.GetFieldValue(CommonUtility.GetKeyAttribute<T>())?.ToString();

            foreach (IFormFile file in files)
            {
                if (file.Length == 0) { continue; }

                (string folderPath, string savePath) = GetFileInfo(category, file.FileName, keyValue);

                List<string> splited = file.Name.Replace("]", "").Split('[').ToList();
                model = await SyncFormValueToModel(model, splited, savePath);
            }
        }

        public (string, string) GetFileInfo(string category, string fileName, string id = "")
        {
            if (string.IsNullOrEmpty(id)) { id = Guid.NewGuid().ToString(); }

            string targetFolder = $"{StandardizePath(category)}/{SystemVariable.Now:yyyy-MM}/{id}";
            string folderPath = @$"{_uploadFilePath}/Uploads/{targetFolder}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string savePath = $@"/Uploads/{targetFolder}/{fileName}";

            return (folderPath, savePath);
        }

        private async Task<T> SyncFormValueToModel<T>(T model, List<string> splited, string value) where T : new()
        {
            //例 檔案名稱: FileList[2][Path]，參考至max的SetFileValueAsync，
            //新的axios有把上傳機制修正，因此做此改動。
            model ??= new T();

            if (splited.Count > 0)
            {
                string fieldName = splited[0];
                splited.RemoveAt(0);

                //如果陣列是數字
                if (int.TryParse(fieldName, out int index))
                {
                    IList list = model as IList;
                    list[index] = await SyncFormValueToModel(list[index], splited, value);
                }
                else //如果陣列是字串
                {
                    PropertyInfo property = model.GetType().GetProperty(fieldName);
                    object newValue;

                    //如果splited沒有下一個等級，則套用value這個值
                    if (splited.Count == 0) { newValue = value; }
                    else { newValue = await SyncFormValueToModel(property.GetValue(model), splited, value); }

                    property.SetValue(model, newValue);
                }
            }
            return model;
        }

        public async Task SaveModelFilesAsync<T>(T model, IFormFileCollection files, string targetFolder) where T : new()
        {
            foreach (IFormFile file in files)
            {
                if (file.Length == 0) { continue; }

                var levels = file.Name.Split('.');
                model = await SetFileValueAsync(model, levels, CreateUploadFileAsync(file, targetFolder));
            }
        }

        private async Task<T> SetFileValueAsync<T>(T model, IEnumerable<string> levels, Task<UploadFile> value) where T : new()
        {
            model ??= new T();
            string currentLevel = levels.First();
            string[] splited = currentLevel.Trim(']').Split('[');
            PropertyInfo property = model.GetType().GetProperty(splited?.First());

            if (int.TryParse(splited.Last(), out int index))
            {
                IList list = property.GetValue(model) as IList;

                if (levels.Count() == 1)
                {
                    list[index] = await value;
                    property.SetValue(model, list);
                }
                else
                {
                    list[index] = await SetFileValueAsync(list[index], levels.Skip(1), value);
                    property.SetValue(model, list);
                }
            }
            else
            {
                if (property != null)
                {
                    if (levels.Count() == 1)
                    {
                        property.SetValue(model, await value);
                    }
                    else
                    {
                        var childModel = await SetFileValueAsync(property.GetValue(model), levels.Skip(1), value);
                        property.SetValue(model, childModel);
                    }
                }
            }
            return model;
        }

        public async Task<UploadFile> CreateUploadFileAsync(IFormFile file, string targetFolder)
        {
            var uploadFile = SaveFile(file, targetFolder);
            await InsertAsync(uploadFile);
            return uploadFile;
        }

        public UploadFile SaveFile(IFormFile file, string targetFolder)
        {
            targetFolder = $"{StandardizePath(targetFolder)}/{SystemVariable.Now:yyyy-MM}";

            var id = Guid.NewGuid();
            string folderPath = @$"{_uploadFilePath}/Uploads/{targetFolder}";
            string newFileName = $"{id}{Path.GetExtension(file.FileName)}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using var stream = new FileStream($"{folderPath}/{newFileName}", FileMode.Create);
            file.CopyTo(stream);

            return new UploadFile
            {
                UploadFileId = id,
                Url = $"/Uploads/{targetFolder}/{newFileName}",
                ContentType = file.ContentType,
                OriginalFileName = file.FileName,
            };
        }

        private static string StandardizePath(string path)
        {
            return path.Trim('\\', '/').Replace('\\', '/');
        }
    }
}
