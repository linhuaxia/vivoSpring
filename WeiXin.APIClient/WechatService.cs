using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tool;
using Vivo.IBLL;
using Vivo.Model;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;
using Vivo.APIClient;
using WeiXin.APIClient.WechatMsgHander;

namespace WeiXin.APIClient
{

    public static partial class WechatService
    {

        static DateTime TimeGetJsTicket = DateTime.Now;
        static string JsTicket = string.Empty;



        /// <summary>
        /// 获取access token
        /// </summary>
        /// <returns>成功时返回access_token，失败时则返回错误码，可通过判断其返回字符串长度来断定是错误码还是access token</returns>
        public static string GetAccessTonken(bool UpdateNew)
        {
            
            return new WechatClient().GetAccessTonken();
        }
        public static string GetAccessTonken()
        {
            return GetAccessTonken(false);
        }


        /// <summary>
        /// 随机串
        /// </summary>
        public static string getNoncestr()
        {
            Random random = new Random();
            return GetMD5(random.Next(1000).ToString(), "GBK").ToLower().Replace("s", "S");
        }

        #region JS API

        public static WechatJSconfigInfo GetWechatJSconfigInfo(string url)
        {
            WechatJSconfigInfo info = new WechatJSconfigInfo();
            info.timestamp = ConvertDateTimeInt(DateTime.Now).ToString();
            info.nonceStr = getNoncestr();
            info.signature = "jsapi_ticket=" + GetJsTicket();
            info.signature += "&noncestr=" + info.nonceStr + "&timestamp=" + info.timestamp + "&url=" + url;
            info.signature = FormsAuthentication.HashPasswordForStoringInConfigFile(info.signature, "SHA1");
            info.jsApiList = new List<string>();
            #region jsApiList SetValue
            info.jsApiList.Add("checkJsApi");
            info.jsApiList.Add("onMenuShareTimeline");
            info.jsApiList.Add("onMenuShareAppMessage");
            info.jsApiList.Add("onMenuShareQQ");
            info.jsApiList.Add("onMenuShareWeibo");
            info.jsApiList.Add("hideMenuItems");
            info.jsApiList.Add("showMenuItems");
            info.jsApiList.Add("hideAllNonBaseMenuItem");
            info.jsApiList.Add("showAllNonBaseMenuItem");
            info.jsApiList.Add("translateVoice");
            info.jsApiList.Add("startRecord");
            info.jsApiList.Add("stopRecord");
            info.jsApiList.Add("onRecordEnd");
            info.jsApiList.Add("playVoice");
            info.jsApiList.Add("pauseVoice");
            info.jsApiList.Add("stopVoice");
            info.jsApiList.Add("uploadVoice");
            info.jsApiList.Add("downloadVoice");
            info.jsApiList.Add("chooseImage");
            info.jsApiList.Add("previewImage");
            info.jsApiList.Add("uploadImage");
            info.jsApiList.Add("downloadImage");
            info.jsApiList.Add("getNetworkType");
            info.jsApiList.Add("openLocation");
            info.jsApiList.Add("getLocation");
            info.jsApiList.Add("hideOptionMenu");
            info.jsApiList.Add("showOptionMenu");
            info.jsApiList.Add("closeWindow");
            info.jsApiList.Add("scanQRCode");
            info.jsApiList.Add("chooseWXPay");
            info.jsApiList.Add("openProductSpecificView");
            info.jsApiList.Add("addCard");
            info.jsApiList.Add("chooseCard");
            info.jsApiList.Add("openCard'");

            #endregion
            return info;

        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception ex)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }


        private static string GetJsTicket()
        {
            if (!string.IsNullOrEmpty(JsTicket) && (DateTime.Now - TimeGetJsTicket).TotalHours < 1)
            {
                return JsTicket;
            }
            string URL = string.Format(WeiXinConst.JsTicketURL, GetAccessTonken());
            string Result = DataHelper.GetHttpData(URL);
            Dictionary<string, object> dic = JSONHelper.DataRowFromJSON(Result);
            if (dic.Keys.Contains("ticket"))
            {
                JsTicket = dic["ticket"].ToString();
                TimeGetJsTicket = DateTime.Now;
            }
            return JsTicket;

        }

        #endregion


        public static class ConstKey
        {
            public static readonly string 订阅 = "subscribe";
            public static readonly string 取消订阅 = "unsubscribe";
            public static readonly string 没有合适返回消息 = "keyNoKeyResponse";
        }
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 发送主推文本消息
        /// </summary>
        /// <returns>发送成功则返回空字符串，否则返回错误信息</returns>
        public static string ResponseSendMsgText(string OpenID, string MsgContent)
        {
            if (string.IsNullOrEmpty(OpenID))
            {
                return "OpenID不存在";
            }

            string AccessToken = GetAccessTonken();
            string URL = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + AccessToken;
            string PostJson = "{\"touser\":\"((OPENID))\",\"msgtype\":\"text\",\"text\":{\"content\":\"((content))\"}}";
            PostJson = PostJson.Replace("((content))", MsgContent);

            string PostFinalText = PostJson.Replace("((OPENID))", OpenID);
            string Result = DataHelper.PostHttpData(URL, PostFinalText);
            if (Result.Contains("\"ok\""))
            {
                return string.Empty;
            }
            if (Result.Contains("response out of time limit"))
            {
                return "客服回复超过限制";
            }
            if (Result.Contains("40003"))
            {
                return "无法识别的用户ID";
            }
            return Result;
        }



        /// <summary>
        /// 微信公众平台操作类
        /// </summary>
        public class weixin
        {
            private string Token = WeiXinConst.Token; //换成自己的token
            public void Auth()
            {
                string echoStr = System.Web.HttpContext.Current.Request.QueryString["echoStr"];
                if (CheckSignature()) //校验签名是否正确
                {
                    if (!string.IsNullOrEmpty(echoStr))
                    {
                        System.Web.HttpContext.Current.Response.Write(echoStr); //返回原值表示校验成功
                        System.Web.HttpContext.Current.Response.End();
                    }
                }
            }


            public string Handle(string postStr)
            {
                WriteToLog(postStr);
                #region 靓仔地实现
                
                WechatMsgBase msgHander = WechatMsgFactory.CreateWechatMsg(postStr);
                if (null==msgHander)
                {
                    return "Error";
                }
               

                string Result = msgHander.Handle();
                return Result;
                #endregion

            }


            /// <summary>
            /// 验证微信签名
            /// * 将token、timestamp、nonce三个参数进行字典序排序
            /// * 将三个参数字符串拼接成一个字符串进行sha1加密
            /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
            /// </summary>
            /// <returns></returns>
            public bool CheckSignature()
            {
                string signature = System.Web.HttpContext.Current.Request.QueryString["signature"];
                string timestamp = System.Web.HttpContext.Current.Request.QueryString["timestamp"];
                string nonce = System.Web.HttpContext.Current.Request.QueryString["nonce"];
                //加密/校验流程：
                //1. 将token、timestamp、nonce三个参数进行字典序排序
                string[] ArrTmp = { Token, timestamp, nonce };
                Array.Sort(ArrTmp);//字典排序
                                   //2.将三个参数字符串拼接成一个字符串进行sha1加密
                string tmpStr = string.Join("", ArrTmp);
                tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
                tmpStr = tmpStr.ToLower();
                //3.开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
                if (tmpStr == signature)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            

            /// <summary>
            /// 将本次交互信息保存至数据库中
            /// </summary>
            /// <param name="requestXML"></param>
            /// <param name="_xml"></param>
            /// <param name="_pid"></param>
            private void WriteToLog(string postStr)
            {
                LogInfo info = new LogInfo();
                info.Category = "GetWechatMsgLog";
                info.UserName = "System";
                info.Detail = postStr;
                info.AddDate = DateTime.Now;
                info.Serious = 1;
                info.UserName = "";
                //new LogInfoService().Create(info);

            }
        }
    }
}

