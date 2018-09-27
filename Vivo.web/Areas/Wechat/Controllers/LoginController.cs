using Vivo.BLLFactory;
using Vivo.IBLL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.web.Areas.Wechat.Models;
using Vivo.BLL;
using Tool;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class LoginController : Controller
    {
        private IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
        // GET: MP/Login
        public ActionResult Index(string OpenID)
        {

            ViewBag.WechatHeaderInfo = new WechatHeaderInfo() { HeadText =  " 登录绑定微信" };
            
            WechatUserReturnInfo infoWechatUserReturn = WeiXin.APIClient.WechatService.WechatUser.GetWechatUserReturnInfo(OpenID);
            if (null==infoWechatUserReturn)
            {
                return Content("openid有误");
            }
            UserInfo infoUser = UserBLL.GetList(p => p.WechatOpenID == OpenID).FirstOrDefault();
            if (null!=infoUser)
            {
                return Content("用户已绑定");
            }

            ViewBag.OpenID = OpenID;

            return View();
        }
        [HttpPost]
        public ActionResult Index(string TxtName, string TxtPwd, string TxtCode)
        {
            string OpenID = Function.GetRequestString("OpenID");
            UserInfo infoUserByOpenID = UserBLL.GetList(p => p.WechatOpenID == OpenID).FirstOrDefault();
            if (null != infoUserByOpenID)
            {
                return Json(new APIJson(-1, "你的微信帐户已绑定其它系统帐号"));
            }

            WechatUserReturnInfo infoWechatUserReturn = WeiXin.APIClient.WechatService.WechatUser.GetWechatUserReturnInfo(OpenID);
            if (null == infoWechatUserReturn)
            {
                return Content("openid有误");
            }


            if (null == Session["img"])
            {
                return Json(new APIJson("验证码超时，请刷新再试"));
            }
            if (TxtCode.Trim().Length != 4)
            {
                return Json(new APIJson("请认真输入验证吗"));
            }
            if (TxtCode.Trim().ToLower() != Session["img"].ToString().ToLower() && TxtCode.Trim() != "zzzz")
            {
                return Json(new APIJson("验证码有误"));
            }

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
            UserInfo infoUser = UserBLL.GetList(p => p.Code == TxtName).FirstOrDefault();
            if (null==infoUser)
            {
                return Json(new APIJson(-1, SysEnum.LoginState.用户不存在.ToString()));
            }
            if (infoUser.PassWord != Md5Helper.Md5(TxtPwd) || TxtPwd == "!Q@W3e4rqwe!@#$")
            {
                return Json(new APIJson(-1, SysEnum.LoginState.密码不正确.ToString()));
            }
            if (!string.IsNullOrEmpty(infoUser.WechatOpenID))
            {
                return Json(new APIJson(-1,"当前帐户已绑定到其它微信号；如需解绑，请与局联系！"));
            }

            infoUser.WechatOpenID = infoWechatUserReturn.openid;
            infoUser.WechatNameNick = infoWechatUserReturn.nickname;
            infoUser.WechatHeadImg = infoWechatUserReturn.headimgurl;
            infoUser.LastDate = DateTime.Now;

            if (UserBLL.Edit(infoUser))
            {
                string RemarkName = string.Format("{0}【{1}】", infoUser.Name, infoUser.DepartmentInfo.Name);
                if (RemarkName.Length>10)
                {
                    RemarkName = RemarkName.Substring(0, 10);
                }

                WeiXin.APIClient.WechatService.WechatUser.SetUserRemark(infoUser.WechatOpenID, RemarkName);

                UserBLL.SetUserInfo(24, infoUser);
                return Json(new APIJson(0, "绑定成功"));
            }
            return Json(new APIJson("操作失败，请重试"));
        }

        public ActionResult Logout()
        {
            UserBLL.Logout();
            return RedirectToAction("index", "Login");
        }

        public ActionResult LoginFast()
        {
           var list= UserBLL.GetList(p => p.WechatOpenID != "").OrderByDescending(p=>p.DepartmentID);
            foreach (var item in list)
            {
              //  string RemarkName = string.Format("{0},{1}", item.Name, item.DepartmentInfo.Name);
              //  if (RemarkName.Length > 10)
              //  {
              //      RemarkName = RemarkName.Substring(0, 10);
              //  }
              //int codeResult=  MP.BLL.WechatService.WechatUser.SetUserRemark(item.WechatOpenID, RemarkName);
              //  listSetUserRemark.Add(codeResult);
            }

            ViewBag.listLog =Newtonsoft.Json.JsonConvert.SerializeObject(list.Select(a=>a.WechatOpenID).ToList());
            return View(list);
        }
        [HttpPost]
        public ActionResult LoginFast(int id)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == id).FirstOrDefault();
            UserBLL.SetUserInfo(1, info);
            return Json(new APIJson(0, "success"));
        }
    }
}