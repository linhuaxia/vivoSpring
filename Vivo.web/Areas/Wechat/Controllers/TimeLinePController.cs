using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Vivo.IBLL;
using Vivo.BLLFactory;
using Vivo.Model;
using Tool;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class TimeLinePController : MP.Controllers.BaseController
    {
        // GET: Wechat/TimeLineP
        public ActionResult Detail(int ID)
        {
            var info = TimeLineBLL.GetList(a => a.ID == ID).FirstOrDefault();
            return View(info);
        }
    }
}