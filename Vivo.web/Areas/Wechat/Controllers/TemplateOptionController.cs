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
    public class TemplateOptionController : BaseMPController
    {

        public ActionResult Index(int TemplateID)
        {


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "模板选项管理";
            ViewBag.WechatHeaderInfo = infoHead;
            //  GetSelectList();


            var list = TemplateOptionBLL.GetList(p => p.TemplateID==TemplateID);

            list = list.OrderByDescending(p => p.SortID).ThenBy(a => a.ID);
            return View(list);
        }

        public ActionResult Create(int TemplateID)
        {
            TemplateOptionInfo info = new TemplateOptionInfo();
            info.TemplateID = TemplateID;
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        public ActionResult Create(TemplateOptionInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson(0, "请填写名称"));
            }
            
            if (TemplateOptionBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0, "提交成功", new { info.ID}));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        public ActionResult Detail(int id)
        {
            TemplateOptionInfo info = TemplateOptionBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }


        public ActionResult Edit(int id)
        {
            TemplateOptionInfo info = TemplateOptionBLL.GetList(p => p.ID == id).FirstOrDefault();
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TemplateOptionInfo info)
        {
            TemplateOptionInfo infoExist = TemplateOptionBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json("请填写名称");
            }
            
            infoExist.Name = info.Name;
            infoExist.SortID = info.SortID;
            infoExist.Enable = info.Enable;

            if (TemplateOptionBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var info = TemplateOptionBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (TemplateOptionBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

    }
}
