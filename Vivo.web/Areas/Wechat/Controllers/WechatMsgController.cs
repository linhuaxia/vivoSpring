using Vivo.BLLFactory;
using Vivo.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Vivo.Model;
using Tool;
using System.Data.Entity;
using Vivo.web.Areas.Wechat.Models;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class WechatMsgController : BaseMPController
    {
        public ActionResult Index(int page = 1)
        {
            string Name = Function.GetRequestString("Name");
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");

            var list = WechatMsgBLL.GetList(p => p.Status!=-1);



            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(p => (p.UserInfo.Name.Contains(Name)
                || p.UserInfo.WechatNameNick.Contains(Name)
                || p.UserInfo.WechatOpenID.Contains(Name)
                ||p.UserInfo.Tel.Contains(Name)));
                ViewBag.TxtName = Name;
            }
            if (DateBegin != DicInfo.DateZone)
            {
                list = list.Where(p => DbFunctions.DiffDays(p.AddDate, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd != DicInfo.DateZone)
            {
                list = list.Where(p => DbFunctions.DiffDays(p.AddDate, DateEnd) >= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }

            list = list.OrderByDescending(p => p.ID);
            IPagedList<WechatMsgInfo> result = list.ToPagedList(page, PageSize);

            WechatHeaderInfo infoHead = new WechatHeaderInfo() { HeadText = "微信会话管理" };
            infoHead.LeftIcon = "/Content/wechat/images/item.png";
            infoHead.LeftURL = "javascript:Seach_Toggle();";
            infoHead.RightText = Url.Action("index", "home");
            ViewBag.WechatHeaderInfo = infoHead;


            return View(result);
        }

        public ActionResult Talk(string FromUserName,int page=1)
        {
            var list = WechatMsgBLL.GetList(p => p.FromUserName == FromUserName|| p.ToUserName==FromUserName).OrderByDescending(p=>p.AddDate);
            if (list.Count()==0)
            {
                return RedirectToAction("E502","Error",new { Msg="没有任何会话内容"});
            }
            ViewBag.wcUserInfo = WeiXin.APIClient.WechatService.WechatUser.GetWechatUserReturnInfo(FromUserName);

            ViewBag.WechatHeaderInfo = new WechatHeaderInfo() { HeadText = "信息会话" };
            IPagedList<WechatMsgInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(WechatMsgInfo info)
        {
           string Result= WeiXin.APIClient.WechatService.ResponseSendMsgText(info.ToUserName, info.Content);
            if (string.IsNullOrEmpty(Result))
            {
                info.CreateUserID = CurrentUser.ID;
                info.AddDate = DateTime.Now;
                info.Status = -1;
                info.FromUserName = string.Empty;
                info.CreateTime = 0;
                info.MsgType = "text";
                info.MsgId = 0;
                info.XMLDom = string.Empty;
                WechatMsgBLL.Create(info);
                return Json(new APIJson(0, "回复成功"));
            }
            return Json(new APIJson(-1, Result));
        }
    }
}
