using Eval.BLLFactory;
using Eval.IBLL;
using Eval.Model;
using Tool;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Linq;

namespace Eval.API.Controllers
{
    [CustomerAuthorize]
    public class BaseController : ApiController
    {
        protected IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
        protected UserInfo CurrentUserInfo;
        public BaseController()
        {
            // CurrentUser = UserBLL.GetCurrent();
            int UserID = Function.GetRequestInt("UserID");
            CurrentUserInfo = UserBLL.GetList(p => p.ID == UserID).FirstOrDefault();
        }


    }

    public class CustomerAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>();
            bool isAnonymous = attributes.Any(a => a is AllowAnonymousAttribute);
            if (isAnonymous)
            {
                base.OnAuthorization(actionContext);
                return;
            }


            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
            int UserID = Function.GetRequestInt("UserID");
            UserInfo CurrentUserInfo = UserBLL.GetList(p => p.ID == UserID).FirstOrDefault();
            if (CurrentUserInfo == null)
            {
                HandleUnauthorizedRequest(actionContext);
                return;
            }

        }
    }
}