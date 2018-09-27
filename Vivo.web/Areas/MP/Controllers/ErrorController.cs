using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;

namespace Vivo.web.Areas.MP.Controllers
{
    public class ErrorController : Controller
    {
        // GET: MP/Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PowerLimit(string Msg)
        {
            if (string.IsNullOrEmpty(Msg))
            {
                Msg = "Error";
            }
            return Json(new APIJson(Msg), JsonRequestBehavior.AllowGet);
        }
    }
}