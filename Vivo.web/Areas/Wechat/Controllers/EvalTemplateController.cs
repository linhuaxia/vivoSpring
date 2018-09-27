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
    public class EvalTemplateController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "评价模板管理";
            ViewBag.WechatHeaderInfo = infoHead;
            //  GetSelectList();


            var list = EvalTemplateBLL.GetList(p => true);

            list = list.OrderByDescending(p => p.SortID).ThenByDescending(a => a.ID);
            IPagedList<EvalTemplateInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            ViewBag.listTypeFlag = Common.EnumToSelectListItem(typeof(SysEnum.TemplateTypeFlag));
            ViewBag.listSort = Common.GetListOrderID();
            return View();
        }
        [HttpPost]
        public ActionResult Create(EvalTemplateInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json("请填写模板名称");
            }
            if (info.Amin < 0 || info.BMin < 0 || info.CMin < 0 || info.DMin < 0)
            {
                return Json("下限值不能小于0");
            }
            if (EvalTemplateBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0, "提交成功", new { info.ID}));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        public ActionResult Detail(int id)
        {
            EvalTemplateInfo info = EvalTemplateBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }


        public ActionResult Edit(int id)
        {
            EvalTemplateInfo info = EvalTemplateBLL.GetList(p => p.ID == id).FirstOrDefault();
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EvalTemplateInfo info)
        {
            EvalTemplateInfo infoExist = EvalTemplateBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json("请填写模板名称");
            }
            if (info.Amin < 0 || info.BMin < 0 || info.CMin < 0 || info.DMin < 0)
            {
                return Json("下限值不能小于0");
            }
            infoExist.Name = info.Name;
            infoExist.SortID = info.SortID;
            infoExist.Enable = info.Enable;
            infoExist.Amin = info.Amin;
            infoExist.BMin = info.BMin;
            infoExist.CMin = info.CMin;
            infoExist.DMin = info.DMin;
            if (EvalTemplateBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            var info = EvalTemplateBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (EvalTemplateBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }


    }
}
