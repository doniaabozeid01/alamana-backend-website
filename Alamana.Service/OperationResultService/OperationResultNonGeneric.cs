using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamana.Service.OperationResultService
{
    public class OperationResultNonGeneric
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static OperationResultNonGeneric Successed(string msg = "") => new OperationResultNonGeneric { Success = true, Message = msg };
        public static OperationResultNonGeneric Fail(string msg) => new OperationResultNonGeneric { Success = false, Message = msg };

    }
}
