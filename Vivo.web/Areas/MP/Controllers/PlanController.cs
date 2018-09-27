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

namespace Vivo.web.Areas.MP.Controllers
{
    public class PlanController : BaseController
    {

        public ActionResult Index(int page = 1)
        {


            var list = PlanBLL.GetList(p => true);
            list = list.OrderBy(a=>a.AgenterName).ThenBy(a=>a.DateBegin).ThenBy(a=>a.SortID);
            IPagedList<PlanInfo> result = list.ToPagedList(page, 50);
            return View(result);
        }

        public ActionResult Create()
        {
            ViewBag.listPrizeType = Common.EnumToSelectListItem(typeof(SysEnum.PrizeType));
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PlanInfo info)
        {
            info.Enable = true;
            PlanBLL.Create(info);
            if (info.ID > 0)
            {
                return Json(new APIJson(0, "添加成功"));
            }
            return Json(new APIJson(-1, "添加失败"));
        }

        public ActionResult Edit(int id)
        {
            ViewBag.listPrizeType = Common.EnumToSelectListItem(typeof(SysEnum.PrizeType));
            PlanInfo info = PlanBLL.GetList(p => p.ID == id).FirstOrDefault();


            return View(info);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PlanInfo info)
        {
            PlanInfo infoExist = PlanBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (null == infoExist)
            {
                return Json(new APIJson(-1, "parms error"));
            }
            info.Enable = true;
            infoExist.DateBegin = info.DateBegin;
            infoExist.DateEnd = info.DateEnd;
            infoExist.AgenterName = info.AgenterName;
            infoExist.Mount = info.Mount;
            infoExist.Enable = info.Enable;
            infoExist.MarketLevel = info.MarketLevel;
            infoExist.TypeFlag = info.TypeFlag;
            infoExist.SortID = info.SortID;
            if (PlanBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功", info));
            }
            return Json(new APIJson(-1, "提交失败", info));
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            PlanInfo info = PlanBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (PlanBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));
            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

    }
}
