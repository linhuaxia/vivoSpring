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
    public class TemplateItemController : BaseMPController
    {

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(TemplateItemInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson(-1, "请填写项目名称"));
            }
            EvalTemplateInfo infoTemplate = EvalTemplateBLL.GetList(a => a.ID == info.TemplateID).FirstOrDefault();
            if (null==infoTemplate)
            {
                return Json(new APIJson(-1, "parms Error"));
            }

            if (infoTemplate.TypeFlag==(int)SysEnum.TemplateTypeFlag.分值模板)
            {
                if (info.MaxScore <= 0)
                {
                    return Json(new APIJson(-1, "最大分值是0，你让人家怎么填真是0分吗？"));
                }

            }
            TemplateItemInfo infoExist = TemplateItemBLL.GetList(a => a.ID == info.ID).FirstOrDefault();
            if (null!=infoExist)
            {
                infoExist.Name = info.Name;
                infoExist.MaxScore = info.MaxScore;
                infoExist.SortID = info.SortID;
                infoExist.Enable = info.Enable;
                if (TemplateItemBLL.Edit(infoExist))
                {
                    return Json(new APIJson(0,"更新成功"));
                }
                return Json(new APIJson("更新失败，请重试"));
            }

            if (TemplateItemBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0,"添加成功"));
            }
            return Json(new APIJson("添加失败，请重试"));
        }

        [HttpPost]
        public ActionResult Delete(int ID)
        {
            var info = TemplateItemBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null!=info)
            {
                TemplateItemBLL.Delete(info);
            }
            return Json(new APIJson(0, "删除成功"));
        }


    }
}
