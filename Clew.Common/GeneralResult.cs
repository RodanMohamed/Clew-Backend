using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clew.Common
{
    public class GeneralResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<Errors>>? Errors { get; set; }
        public static GeneralResult SuccessResult(string message = "Success")
            => new() { Success = true, Message = message, Errors = null };

        public static GeneralResult NotFound(string message = "Resource not found.")
            => new() { Success = false, Message = message, Errors = null };

        public static GeneralResult FailResult(string message = "Operation faild.")
            => new() { Success = false, Message = message, Errors = null };

        public static GeneralResult FailResult(Dictionary<string, List<Errors>> errors, string message = "One or more validation errors occurred.")
            => new() { Success = false, Message = message, Errors = errors };
    }
    public class GeneralResult<T> : GeneralResult
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T Data { get; set; }

        public static GeneralResult<T> SuccessResult(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data, Errors = null };

        public static new GeneralResult<T> SuccessResult(string message = "Success")
             => new() { Success = true, Message = message, Errors = null };

        public static new GeneralResult<T> NotFound(string message = "Resource not found")
            => new() { Success = false, Message = message, Errors = null };

        public static new GeneralResult<T> FailResult(string message = "Operation faild.")
            => new() { Success = false, Message = message, Errors = null };

        public static new GeneralResult<T> FailResult(Dictionary<string, List<Errors>> errors, string message = "One or more validation errors occurred.")
            => new() { Success = false, Message = message, Errors = errors };
    }

}

