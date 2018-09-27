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

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class PlanCategoryController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {
            var list = PlanCategoryBLL.GetList(p =>  p.ID != Wechat.Controllers.ResearchPersonController.PlanCategoryPersonalID);
            list = list.OrderByDescending(p => p.Enable ).ThenByDescending(p=>p.ID);
            IPagedList<PlanCategoryInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int id)
        {
            PlanCategoryInfo info = PlanCategoryBLL.GetList(p => p.ID == id).FirstOrDefault();


            return View(info);
        }

        [AllowAnonymous]
        public ActionResult Detail(int id)
        {
            PlanCategoryInfo info = PlanCategoryBLL.GetList(p => p.ID == id).FirstOrDefault();


            return View(info);
        }


    }
}
