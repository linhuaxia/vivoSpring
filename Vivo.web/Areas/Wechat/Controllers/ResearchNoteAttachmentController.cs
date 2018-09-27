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
    public class ResearchNoteAttachmentController : BaseMPController
    {


        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult Create(ResearchNoteAttachmentInfo info)
        {
            ResearchNoteInfo infoResearchNote = ResearchNoteBLL.GetList(a => a.ID == info.ResearchNoteID).FirstOrDefault();
            if (null==infoResearchNote)
            {
                return Json(new APIJson(-1, "数据有误，找不到课堂记录对象"));
            }
            if (infoResearchNote.ResearchInfo.Status == (int)SysEnum.ResearchStatus.已确认)
            {
                return Json(new APIJson(-1,"当前状态不能上传"));
            }
            if (string.IsNullOrEmpty(info.MineType))
            {
                info.MineType = "image";
            }
            string SavePathRelative = SaveWechatImage(info.Name, info.MineType, infoResearchNote.ResearchInfo);
            info.Name = SavePathRelative.Substring(SavePathRelative.LastIndexOf("/") + 1);
            info.PathRelative= SavePathRelative.Substring(0,SavePathRelative.LastIndexOf("/")+1);
            info.CreateDate = DateTime.Now;
            
            if (ResearchNoteAttachmentBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0, "提交成功", ResearchNoteAttachmentBLL.GetImageJSON(infoResearchNote,info.MineType)));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Delete(int ID)
        {
            ResearchNoteAttachmentInfo info = ResearchNoteAttachmentBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null==info)
            {
                return Json(new APIJson("数据不存在"));
            }
            var infoResearch = info.ResearchNoteInfo.ResearchInfo;
            int ResearchNoteID = info.ResearchNoteID;
            if (infoResearch.Status == (int)SysEnum.ResearchStatus.已确认)
            {
                return Json(new APIJson(-1, "当前状态不能上传"));
            }
            string MineType = info.MineType;
            try
            {
                System.IO.File.Delete(Server.MapPath(info.PathRelative + info.Name));
            }
            catch (Exception){}
            if (ResearchNoteAttachmentBLL.Delete(info))
            {
                var infoResearchNoteDb = ResearchNoteBLL.GetList(a => a.ID == ResearchNoteID).FirstOrDefault();
                var result = new {
                    listImage= ResearchNoteAttachmentBLL.GetImageJSON(infoResearchNoteDb, "image"),
                    listAudio = ResearchNoteAttachmentBLL.GetImageJSON(infoResearchNoteDb, "audio")
                };
                return Json(new APIJson(0,"删除成功", result));
            }
            return Json(new APIJson("删除失败，请重试"));
        }
        public const string ImageSavePathRelative = "/Content/file/ResearchPlan/Research/";

        private string SaveWechatImage(string MediaID,string MineType, ResearchInfo infoResearch)
        {
            string RelativePath = ImageSavePathRelative + infoResearch.ResearchPlanInfo.DateBegin.ToString("yyyyMM") + "/";
            string SaveMapPath = Server.MapPath(RelativePath);
            if (!Directory.Exists(SaveMapPath))
            {
                Directory.CreateDirectory(SaveMapPath);
            }
            string Ext = ".jpg";
            if (MineType.ToLower().Contains("audio"))
            {
                Ext = ".amr";
            }
            string SaveName = MediaID + Ext;

            WeiXin.APIClient.WechatService.WechatFile.GetMultimedia(WeiXin.APIClient.WechatService.GetAccessTonken(),
                MediaID, SaveMapPath, SaveName);
            if (MineType.ToLower().Contains("audio"))
            {
                Tool.MediaHelper.AmrToMp3Async(SaveMapPath, MediaID);
                SaveName = MediaID + ".mp3";
            }
            return RelativePath + SaveName;
        }


    }
}
