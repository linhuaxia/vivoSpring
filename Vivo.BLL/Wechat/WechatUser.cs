using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tool;
using Eval.IBLL;
using Eval.Model;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;


namespace Eval.BLL
{
    public static partial class WechatService
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        public static class WechatUser
        {
            public static WechatUserReturnInfo GetWechatUserReturnInfo(string OpenID)
            {
                string Token = GetAccessTonken();
                string URL = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", Token, OpenID);
                string JsonResult = DataHelper.GetHttpData(URL);
                WechatUserReturnInfo info = new WechatUserReturnInfo();
                if (JsonResult.IndexOf("errcode") < 0)
                {
                    info = Newtonsoft.Json.JsonConvert.DeserializeObject<WechatUserReturnInfo>(JsonResult);
                }
                return info;
            }
            /// <summary>
            /// 设置备注名
            /// </summary>
            /// <param name="OpenID"></param>
            /// <param name="Remark"></param>
            /// <returns></returns>
            public static int SetUserRemark(string OpenID, string Remark)
            {
                string Token = GetAccessTonken();
                string URL = string.Format("https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token={0}", Token);

                var postData = new
                {
                    openid = OpenID,
                    remark = Remark
                };
                string postDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
                string JsonResult = DataHelper.PostHttpData(URL, postDataJson);
                Dictionary<string, object> dic = JSONHelper.DataRowFromJSON(JsonResult);
              return  Function.ConverToInt(dic["errcode"].ToString());

            }
        }

    }
}
