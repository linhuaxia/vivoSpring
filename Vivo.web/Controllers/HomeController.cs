using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;

namespace Vivo.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.infoWechatUserReturn = infoWechatUserReturn;
            return View();
        }

        //public ActionResult GetInfoWechatUserReturn()
        //{
        //    return Json(new APIJson(0, "", infoWechatUserReturn));
        //}

    }
}