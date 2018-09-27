using Vivo.BLLFactory;
using Vivo.IBLL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tool;

namespace Vivo.web.Areas.MP.Controllers
{
    public class LoginController : Controller
    {
        private IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
        // GET: MP/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string TxtName, string TxtPwd, string TxtCode)
        {
            TxtName = TxtName.Trim();
            TxtPwd = TxtPwd.Trim();
            TxtCode = TxtCode.Trim();
            //if (null == Session["img"])
            //{
            //    return Json(new APIJson("验证码超时，请刷新再试"));
            //}
            //if (TxtCode.Trim().Length != 4)
            //{
            //    return Json(new APIJson("请认真输入验证吗"));
            //}
            //if (TxtCode.Trim().ToLower() != Session["img"].ToString().ToLower() && TxtCode.Trim() != "zzzz")
            //{
            //    return Json(new APIJson("验证码有误"));
            //}

            if (string.IsNullOrEmpty(TxtName.Trim()))
            {
                return Json(new APIJson("请输入帐号！"));
            }
            if (String.IsNullOrEmpty(TxtName))
            {
                return Json(new APIJson("账号不能为空！"));
            }
            if (String.IsNullOrEmpty(TxtPwd))
            {
                return Json(new APIJson("密码不能为空！"));
            }
            if (TxtName == "admin" && TxtPwd == "vivo123456")
            {
                CookiesHelper.AddCookie("Employee", Md5Helper.Md5Encrypt("sharp_" + 1.ToString()), DateTime.Now.AddHours(24));
                CookiesHelper.AddCookie("EmployeeName", Md5Helper.Md5Encrypt("sharp_" + "1"), DateTime.Now.AddHours(24));
                System.Web.HttpContext.Current.Session["UserInfo"] = new UserInfo() { ID = 1, WechatOpenID = "1" };
                return Json(new APIJson(0, "success"));
            }
            return Json(new APIJson(0, "登录失败"));

        }

    }
}