using Vivo.BLLFactory;
using Vivo.IBLL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vivo.web.Areas.MP.Controllers 
{

    public class BaseMPUnFilterController : BaseController
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            CurrentUser = UserBLL.GetCurrent();
            if (null == CurrentUser)
            {
                
                filterContext.Result = RedirectToAction("index", "Login");
                return;
            }

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

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var result = new APIJson(-502, "Oauth deny");
                filterContext.Result = new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}