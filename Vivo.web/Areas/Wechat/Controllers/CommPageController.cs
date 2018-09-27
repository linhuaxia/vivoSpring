using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Vivo.web.Areas.Wechat.Models;
using System.Dynamic;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class CommPageController : BaseMPUnFilterController
    {
        // GET: Wechat/CommanPage
        public ActionResult Msg(string Title, string GoBackLink="/wechat/home",string Detail="",string Icon="")
        {
            dynamic info = new ExpandoObject();
            info.Title = Title;
            info.Detail = Detail;
            info.Icon = Icon;
            info.GoBackLink = GoBackLink;
            ViewBag.info = info;

            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = Title;
            infoHead.LeftURL = GoBackLink;
            ViewBag.WechatHeaderInfo = infoHead;
            return View();
        }
    }
}