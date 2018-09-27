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
using Vivo.web.Areas.MP.Controllers;
using Vivo.web.Areas.Wechat.Models;
using System.Dynamic;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class DepartmentController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "学校管理";
            infoHead.LeftIcon = "/Content/wechat/images/search.png";
            infoHead.LeftURL = Url.Action("index","Home");
            ViewBag.WechatHeaderInfo = infoHead;
            GetSelectList();

            string Name = Function.GetRequestString("Name");
            int TypeID = Function.GetRequestInt("TypeID");

            var list = DepartmentBLL.GetList(p => true && p.ID!=4);
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(p => p.Name.Contains(Name));
                ViewBag.TxtName = Name;
            }
            if (TypeID > 0)
            {
                list = list.Where(p => p.DepartmentTypeInfo.Any(a=>a.ID == TypeID));
                ViewBag.DdlTypeID = TypeID;
            }

            list = list.OrderByDescending(p => p.ID);
            IPagedList<DepartmentInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            GetSelectList();
            return View();
        }
        [AllowAnonymous]
        public ActionResult OutCreate()
        {
            GetSelectList();
            return View();
        }

        private void GetSelectList()
        {
            ViewBag.OrderID = Common.GetListOrderID();
            ViewBag.listTypeEnum = Common.EnumToSelectListItem(typeof(SysEnum.DepartmentTypeEnum));
            ViewBag.listDepartmentType=DepartmentTypeBLL.GetList(a => a.Enable);
        }

        public ActionResult Edit(int id)
        {
            DepartmentInfo info = DepartmentBLL.GetList(p => p.ID == id).FirstOrDefault();

            GetSelectList();

            ViewBag.listDepartmentTypeJSON =Newtonsoft.Json.JsonConvert.SerializeObject(info.DepartmentTypeInfo.Select(a=>new { a.ID,a.Name}).ToList());

            ViewBag.DdlOrderID = Common.GetListOrderID();


            return View(info);
        }
        public ActionResult Detail(int id)
        {
            DepartmentInfo info = DepartmentBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }

        [AllowAnonymous]
        public ActionResult APIQueue(int AreaID)
        {
            var list = DepartmentBLL.GetList(a => a.TypeEmun == AreaID);
            var result = list.OrderBy(a=>a.Name).ToList().Select(a => new { a.ID, Name = a.Name }).ToList();
            return Json(new APIJson(0,"", result), JsonRequestBehavior.AllowGet);
        }



    }
}
