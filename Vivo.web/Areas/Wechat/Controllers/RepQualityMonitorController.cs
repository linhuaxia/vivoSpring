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
using System.Dynamic;
using System.Data.Entity;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class RepQualityMonitorController : MP.Controllers.BaseMPController
    {

        public ActionResult Index(int page = 1)
        {
            ViewBag.listRepQualityMonitorType = MP.Controllers.Common.EnumToSelectListItem(typeof(SysEnum.RepQualityMonitorType));
            return new Rep.Controllers.RepQualityMonitorController().Index(page);


            var list = RepQualityMonitorBLL.GetList(p => true);
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");

            if (DateBegin > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateBegin, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateEnd, DateEnd) >= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }
            int TypeID = Function.GetRequestInt("TypeID");
            if (TypeID>0)
            {
                list = list.Where(a=>a.TypeFlag==TypeID);
                ViewBag.DdlTypeID = TypeID;
            }
            list = list.OrderByDescending(p => p.ID);
            IPagedList<RepQualityMonitorInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            ViewBag.listRepQualityMonitorType = MP.Controllers.Common.EnumToSelectListItem(typeof(SysEnum.RepQualityMonitorType));
            return View();
        }

        public ActionResult Edit(int id)
        {
            RepQualityMonitorInfo info = RepQualityMonitorBLL.GetList(p => p.ID == id).FirstOrDefault();

            ViewBag.listRepQualityMonitorType = MP.Controllers.Common.EnumToSelectListItem(typeof(SysEnum.RepQualityMonitorType));


            return View(info);
        }


    }
}
