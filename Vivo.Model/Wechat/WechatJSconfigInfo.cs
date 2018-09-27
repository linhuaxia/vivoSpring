using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Vivo.Model
{
    public class WechatJSconfigInfo
    {
        public WechatJSconfigInfo()
        {
            debug = false;
            appId = WeiXinConst.AppId;
        }
        public bool debug { get; set; }
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// 生成签名的时间戳
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// 生成签名的随机串
        /// </summary>
        public string nonceStr { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 需要使用的JS接口列表
        /// </summary>
        public List<string> jsApiList { get; set; }

        public string GetjsApiListString()
        {
            string JS=  string.Join("','", jsApiList);
            JS = JS.Trim('\'');
            JS = "'" + JS + "'";
            return JS;
        }

        public static string GetSignature(string Source_String)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
    }
}
