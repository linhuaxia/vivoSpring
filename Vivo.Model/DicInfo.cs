using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vivo.Model
{
    public static class DicInfo
    {

        public class DicDictionary : Dictionary<int, string>
        {
            public string GetValue(int key)
            {
                return Keys.Contains(key) ? this[key] : string.Empty;
            }
        }


        /// <summary>
        /// 系统能接受的最初日期，1970-01-01
        /// </summary>
        public static readonly DateTime DateZone = new DateTime(1970, 1, 1);
        public const string Admin = "admin";
        public const int AdminID =1;

        public const string ConsoleURL = "http://app.sfc0758.com";
        public const string ConsoleAPIURL = "http://api.sfc0758.com";

        /// <summary>
        /// 学车补交费用2000元
        /// </summary>
        public const decimal LearnDriveRePayFee = 2000;
    }
}