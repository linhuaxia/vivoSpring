using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool
{
    public static class ConfigHelper
    {
        public static readonly string WEBDoMain = "WEBDoMain";
        
        public static class ALIConfig
        { 
        //public static readonly string 支付宝Account = "支付宝Account";
        //public static readonly string 支付宝合作身份者ID = "支付宝合作身份者ID";
        //public static readonly string 支付宝Secret = "支付宝Secret";
        //public static readonly string 支付宝收款方名称 = "支付宝收款方名称";
        //public static readonly string 支付宝公钥public_key = "支付宝公钥public_key";
        //public static readonly string 支付宝私钥private_key = "支付宝私钥private_key";
        }
        public static class CRMConfig
        {
            public static readonly string AppKey = "CrmApiAppKey";
            public static readonly string AppSecret = "CrmApiAppSecret";
            public static readonly string AppToken = "CrmApiAppToken";
            public static readonly string Domain = "CrmApiUrl";

        }





        public static string GetAppendSettingValue(string Key)
        {
            return System.Configuration.ConfigurationSettings.AppSettings[Key.Trim()];
        }
    }
}
