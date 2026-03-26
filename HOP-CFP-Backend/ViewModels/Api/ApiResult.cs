using System;
using System.Collections.Generic;

namespace SmartExpoIoT.ViewModels.Api
{
    /// <summary>
    /// 通用 API 回傳格式
    /// </summary>
    public class ApiResult<T> : BaseResult
    {
        public T Data { get; set; }
    }

    public class BaseResult {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public static class ApiResultExtensions
    {
        public static ApiResult<T> SetSuccess<T>(this ApiResult<T> response, T data, string message = "")
        {
            response.Success = true;
            response.Message = message;
            response.Data = data;
            return response;
        }

        public static ApiResult<T> SetError<T>(this ApiResult<T> response, string message)
        {
            response.Success = false;
            response.Message = message;
            return response;
        }
    }

}
