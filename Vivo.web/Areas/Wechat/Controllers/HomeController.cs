using Vivo.BLLFactory;
using Vivo.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;
using Vivo.web.Areas.Wechat.Models;

namespace Vivo.web.Areas.Wechat.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseMPUnFilterController
    {
        // GET: MP/Home
        public ActionResult Index()
        {

            ViewBag.WechatHeaderInfo = new WechatHeaderInfo() { HeadText = "首页" };
            return View();
        }
        public ActionResult API()
        {
            return View();
        }
    }
}