using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.OperationResultService
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static OperationResult<T> SuccessResult(T data, string message = "")
        {
            return new OperationResult<T> { Success = true, Message = message, Data = data };
        }

        public static OperationResult<T> Fail(string message)
        {
            return new OperationResult<T> { Success = false, Message = message, Data = default };
        }
    }
}
