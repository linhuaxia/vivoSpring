using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Tool;

namespace Vivo.web.Areas.Wechat.Controllers
{

    public class BaseWechatUnUserController : Vivo.web.Areas.MP.Controllers.BaseController
    {
        const string UrlGetToken = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=((APPID))&secret=((SECRET))&code=((CODE))&grant_type=authorization_code";
        const string UrlGetUserInfo = "https://api.weixin.qq.com/sns/userinfo?access_token=((ACCESS_TOKEN))&openid=((OPENID))";
        public BaseWechatUnUserController()
        {

        }

        protected WechatUserReturnInfo infoWechatUserReturn = null;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (null != UserBLL.GetCurrent())
            {
                return;
            }
            if (null != Session["WechatUserReturnInfo"])
            {
                infoWechatUserReturn = (WechatUserReturnInfo)Session["WechatUserReturnInfo"];
            }
            string ReplacedURL = Tool.Function.ReplaceURLParms(Request.Url.AbsoluteUri, "code", "");
            ReplacedURL = ReplacedURL.Replace("code=", "");
            ReplacedURL = ReplacedURL.Replace("code=", "");

            string ErrorContent= string.Format("正在尝试重新登录。请稍候<script>setTimeout('location.href=\"{0}\"',1000);</script>", ReplacedURL);
            if (null == infoWechatUserReturn)
            {
                string WeChatCode = Function.GetRequestString("code"); //这是获取过来的code
                string WeChatState = Function.GetRequestString("STATE");//这个自定义参数未真正使用过

                if (string.IsNullOrEmpty(WeChatCode))   //这是获取过来的code,如果为空，则用户不同意授权
                {
                    #region Step1跑到微信那里去拿个授权登录Code
                    string WechatOauthURL = ConfigHelper.GetAppendSettingValue(ConfigHelper.WEBDoMain) + Request.Url.ToString();
                    string URLgrant = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=((APPID))&redirect_uri=((REDIRECT_URI))&response_type=code&scope=((SCOPE))&state=((STATE))#wechat_redirect";
                    URLgrant = URLgrant.Replace("((APPID))", WeiXinConst.AppId);
                    URLgrant = URLgrant.Replace("((REDIRECT_URI))", System.Web.HttpContext.Current.Server.UrlEncode(Request.Url.ToString()));
                    URLgrant = URLgrant.Replace("((SCOPE))", "snsapi_userinfo");
                    URLgrant = URLgrant.Replace("((STATE))", string.Empty);
                    filterContext.Result = Redirect(URLgrant);
                    return;

                    #endregion
                }

                try
                {
                    infoWechatUserReturn = GetWechatUserInfo(WeChatCode);
                }
                catch (Exception)
                {
                    filterContext.Result = Content(ErrorContent + "A");
                    return;
                }
                if (null == infoWechatUserReturn)
                {
                    filterContext.Result = Content(ErrorContent + "B");
                    return;
                }
                Session["WechatUserReturnInfo"] = infoWechatUserReturn;
            }
            if (null == infoWechatUserReturn)
            {
                filterContext.Result = Content(ErrorContent+"C");

            }
        }




        /// <summary>
        /// Step2 根据Code获取AccessToken和OpenID
        /// </summary>
        /// <param name="WeChatCode"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetAccessToken(string WeChatCode)
        {
            string tokenurl = UrlGetToken.Replace("((APPID))", WeiXinConst.AppId);
            tokenurl = tokenurl.Replace("((SECRET))", WeiXinConst.Secret);
            tokenurl = tokenurl.Replace("((CODE))", WeChatCode);
            string tokenResult = DataHelper.GetHttpData(tokenurl);
            Dictionary<string, object> TokenResultJson = JSONHelper.DataRowFromJSON(tokenResult);
            string js = string.Empty;
            if (TokenResultJson.Keys.Contains("errcode"))
            {
                return null;
            }
            if (!TokenResultJson.Keys.Contains("access_token"))
            {
                return null;
            }

            return TokenResultJson;

        }

        /// <summary>
        /// Step3根据AccessToken获取微信用户详细信息
        /// </summary>
        /// <returns></returns>
        private WechatUserReturnInfo GetWechatUserInfo(string WeChatCode)
        {
            string UserInfoString = string.Empty;
            try
            {
                Dictionary<string, object> TokenResultJson = GetAccessToken(WeChatCode);
                string TokenValue = TokenResultJson["access_token"].ToString();//这是获取过来的token
                string OpenidValue = TokenResultJson["openid"].ToString();//这是获取过来的openId
                string UserInfoURL = UrlGetUserInfo.Replace("((ACCESS_TOKEN))", TokenValue);
                UserInfoURL = UserInfoURL.Replace("((OPENID))", OpenidValue);
                 UserInfoString = DataHelper.GetHttpData(UserInfoURL);


                return Newtonsoft.Json.JsonConvert.DeserializeObject<WechatUserReturnInfo>(UserInfoString);

            }
            catch (Exception ex)
            {
                LogInfo infoLog = new LogInfo();
                infoLog.AddDate = DateTime.Now;
                infoLog.Category = "GetWechatUserInfo";
                infoLog.Detail = UserInfoString + "||Ex=" + ex.Message;
                infoLog.Serious = 1;
                infoLog.UserName = string.Empty;
                LogBLL.Create(infoLog);

                return null;
            }

        }


    }
}