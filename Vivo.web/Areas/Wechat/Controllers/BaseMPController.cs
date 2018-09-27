using Vivo.BLLFactory;
using Vivo.IBLL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tool;
using Eval;
using Vivo.web.Areas.Wechat.Models;

namespace Vivo.web.Areas.Wechat.Controllers
{
    [MPAuthorize]
    public class BaseMPController : BaseMPUnFilterController
    {
        public BaseMPController()
        {

            // infoHead.HeadText = "组织调研";
            //
            ViewBag.WechatHeaderInfo = infoHead;

            // CurrentUser = UserBLL.GetCurrent();
        }




        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (null != filterContext.Result)
            {
                return;
            }
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }


            #region 权限校验

            if (CurrentUser.Name.ToLower() == "admin")
            {
                return;
            }
            if (!CurrentUser.Enable)
            {
                filterContext.Result = Redirect(Url.Action("PowerLimit", "Error", new { Msg = "您的帐户已被禁用，请联系管理员" }));

            }
            string URLShortConsole = URLShort.Replace("/Wechat/", "/MP/");
            string URLFullConsole = URLFull.Replace("/Wechat/", "/MP/");
            List<PowerActionInfo> CurrentPowerActions = PowerActionBLL.GetList(p =>
            (p.Url.Contains(URLFull) || p.Url.Contains(URLFullConsole))
            ).ToList();
            if (null == CurrentPowerActions || CurrentPowerActions.Count == 0)
            {
                filterContext.Result = Redirect(Url.Action("PowerLimit", "Error", new { Msg = "系统不知道有这样的操作权限" }));
                return;
            }

            bool result = CurrentPowerActions.Any(a => PowerActionBLL.PowerCheck(a, CurrentUser));
            if (!result)
            {
                filterContext.Result = Redirect(Url.Action("PowerLimit", "Error", new { Msg = "权限不足，用户已被禁止访问" }));
                return;
            }
            #endregion
        }


    }


    public class BaseMPUnFilterController : BaseWechatUnUserController
    {
        public BaseMPUnFilterController()
        {
            infoHead = new Wechat.Models.WechatHeaderInfo();

            ViewBag.PowerActionBLL = PowerActionBLL;

        }

        public string URLFull;
        public string URLShort;



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);



            var actionDescriptor = filterContext.ActionDescriptor;
            var controllerDescriptor = actionDescriptor.ControllerDescriptor;
            var controller = controllerDescriptor.ControllerName;
            var action = actionDescriptor.ActionName;



            URLShort = "/Wechat/" + controller;
            URLFull = URLShort + "/" + action;
            if (action.ToLower() != "index")
            {
                URLShort = URLFull;
            }

            CurrentUser = UserBLL.GetCurrent();//当前登录用户
            if (null == CurrentUser || string.IsNullOrEmpty(CurrentUser.WechatOpenID))
            {

                if (null== infoWechatUserReturn)
                {
                    return;
                    //throw new Exception("系统无法获取您的微信帐户信息");
                }
                UserInfo infoUserDb = UserBLL.GetList(p => p.WechatOpenID == infoWechatUserReturn.openid).FirstOrDefault();
                if (null == infoUserDb)
                {
                    filterContext.Result = Redirect("/Wechat/Login?OpenID=" + infoWechatUserReturn.openid);
                    return;
                }
                infoUserDb.WechatOpenID = infoWechatUserReturn.openid;
                infoUserDb.WechatNameNick = infoWechatUserReturn.nickname;
                infoUserDb.WechatHeadImg = infoWechatUserReturn.headimgurl;
                UserBLL.SetUserInfo(24, infoUserDb);
                CurrentUser = infoUserDb;
                //一定要set下CurrentUser，不要然继续跳。后果是OauthCode参数也会带上，搞得Code过期
            }
            ViewBag.CurrentUser = CurrentUser;
        }


    }


    public class MPAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();

            UserInfo CurrentUser = UserBLL.GetCurrent();
            return null != CurrentUser;
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // filterContext.HttpContext.Response.Redirect("/Console/User/Login");

            //base.HandleUnauthorizedRequest(filterContext);
        }
    }
}