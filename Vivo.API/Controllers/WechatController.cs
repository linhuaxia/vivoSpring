using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tool;
using Eval.BLLFactory;
using Eval.IBLL;
using Eval.Model;

namespace Eval.API.Controllers
{
    public class WechatController : BaseController
    {
        static DateTime TimeGetToken = DateTime.Now;
        static String WeChat_tokenVal = string.Empty;

        static DateTime TimeGetJsTicket = DateTime.Now;
        static string JsTicket = string.Empty;


        [AllowAnonymous]
        public IHttpActionResult GetAccessTokenByNone(bool WithUpdate=false)
        {
            string Token= GetAccessTonkenHTTP(WithUpdate);
            return Json(new APIJson(0,string.Empty,Token));
        }
        private static string GetAccessTonkenHTTP(bool UpdateNew)
        {
            if (!string.IsNullOrEmpty(WeChat_tokenVal) && (DateTime.Now - TimeGetToken).TotalHours < 1)
            {
                return WeChat_tokenVal;
            }
            string Result = DataHelper.GetHttpData(WeiXinConst.WeiXin_AccessTokenUrl);
            Dictionary<string, object> dic = JSONHelper.DataRowFromJSON(Result);
            if (dic.Keys.Contains("errcode"))
            {
                return dic["errcode"].ToString();
            }
            string TokenResult = dic["access_token"].ToString();
            WeChat_tokenVal = TokenResult;
            TimeGetToken = DateTime.Now;

            return TokenResult;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult testapi()
        {
            string Token = GetAccessTonkenHTTP(false);
            return Json(new APIJson(0, string.Empty, Token));
        }
    }
}
