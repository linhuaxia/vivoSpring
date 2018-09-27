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
using System.IO;
using System.Threading.Tasks;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class ResearchNoteController : BaseMPController
    {
        [AllowAnonymous]
        public ActionResult index(int ResearchID)
        {
            ResearchInfo info = ResearchBLL.GetList(p => p.ID == ResearchID).FirstOrDefault();
            var listResearchNote = info.ResearchNoteInfo.OrderByDescending(a => a.ID).ToList()
                .Select(a => new {
                    a.ID,
                    a.Detail,
                    CreateDate=a.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                    ImageJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "image"),
                    AudioJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "audio")
                });

            return Json(new APIJson(0, "", listResearchNote), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(ResearchNoteInfo info)
        {
            info.Detail = string.Empty;
            info.CreateDate = DateTime.Now;
            ResearchNoteBLL.Create(info);
            return Json(new APIJson(0, "", info.ID));
        }


        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Edit(ResearchNoteInfo info)
        {
            ResearchNoteInfo infoExist = ResearchNoteBLL.GetList(a => a.ID == info.ID).FirstOrDefault();
            if (null== infoExist)
            {
                return Json(new APIJson(-1, "数据有误，找不到课堂记录对象"));
            }
            if (null==info.Detail)
            {
                info.Detail = string.Empty;
            }
            infoExist.Detail = info.Detail;
            if (ResearchNoteBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Delete(int id)
        {
            var info = ResearchNoteBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误"));
            }
            if (ResearchNoteBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功"));

            }
            return Json(new APIJson(-1, "删除失败，请重试"));
        }


    }
}
