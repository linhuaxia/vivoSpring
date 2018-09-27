using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class ErrorController : Controller
    {
        // GET: MP/Error
        public ActionResult Index()
        {
           // Response.StatusCode = 502;
            return View();
        }
        public ActionResult PowerLimit(string Msg)
        {
            if (string.IsNullOrEmpty(Msg))
            {
                Msg = "Error";
            }
            return View(new APIJson(Msg));
        }
        public ActionResult E404(string Msg = "")
        {
            ViewBag.Msg = Msg;
            return View();
        }
        public ActionResult E502(string Msg = "")
        {
            ViewBag.Msg = Msg;
            return View();
        }
    }
}