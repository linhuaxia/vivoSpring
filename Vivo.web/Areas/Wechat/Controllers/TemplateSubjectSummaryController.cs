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
    public class TemplateSubjectSummaryController : BaseMPController
    {

        public ActionResult Index(int TemplateID)
        {


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "学科汇总项管理";
            ViewBag.WechatHeaderInfo = infoHead;
            //  GetSelectList();


            var list = TemplateSubjectSummaryBLL.GetList(p => p.TemplateID==TemplateID);

            list = list.OrderByDescending(p => p.SortID).ThenBy(a => a.ID);
            return View(list);
        }

        public ActionResult Create(int TemplateID)
        {
            TemplateSubjectSummaryInfo info = new TemplateSubjectSummaryInfo();
            info.TemplateID = TemplateID;
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        public ActionResult Create(TemplateSubjectSummaryInfo info)
        {
            if (string.IsNullOrEmpty(info.KeyName))
            {
                return Json(new APIJson(0, "请填写名称"));
            }
            
            if (TemplateSubjectSummaryBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0, "提交成功", new { info.ID}));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        public ActionResult Detail(int id)
        {
            TemplateSubjectSummaryInfo info = TemplateSubjectSummaryBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }


        public ActionResult Edit(int id)
        {
            TemplateSubjectSummaryInfo info = TemplateSubjectSummaryBLL.GetList(p => p.ID == id).FirstOrDefault();
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TemplateSubjectSummaryInfo info)
        {
            TemplateSubjectSummaryInfo infoExist = TemplateSubjectSummaryBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(info.KeyName))
            {
                return Json("请填写名称");
            }
            
            infoExist.KeyName = info.KeyName;
            infoExist.SortID = info.SortID;
            infoExist.Enable = info.Enable;

            if (TemplateSubjectSummaryBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            var info = TemplateSubjectSummaryBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (TemplateSubjectSummaryBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

    }
}
