using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vivo.Model
{
    public class APIJson
    {
        public APIJson(string errorMsg)
        {
            ErrorCode = -1;
            ErrorMsg = errorMsg;
            Data = null;
        }
        public APIJson(Object data)
        {
            ErrorCode = 0;
            ErrorMsg = string.Empty;
            Data = data;
        }
        public APIJson(int errorCode, string errorMsg)
        {
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
            Data = null;
        }
        public APIJson(int errorCode, string errorMsg, Object data)
        {
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
            Data = data;
        }

        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public Object Data { get; set; }

    }
}
