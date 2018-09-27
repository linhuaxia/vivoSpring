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

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class ResearchPlanAttachmentController : BaseMPController
    {

        public ActionResult index(int PlanID)
        {
            ResearchPlanInfo info = ResearchPlanBLL.GetList(p => p.ID == PlanID).FirstOrDefault();
            if (info.DateBegin < DateTime.Now.Date)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "计划已过期" });
            }
            ViewBag.ImageJSON = GetImageJSON(info);// info.ResearchPlanAttachmentInfo.Select(a => new { a.ID, FullPath = a.Name + a.PathRelative });
            return View(info);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult index(ResearchPlanAttachmentInfo info)
        {
            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(a => a.ID == info.ResearchPlanID).FirstOrDefault();
            if (infoPlan.DateBegin.Date<DateTime.Now.Date)
            {
                return Json(new APIJson(-1,"活动已过期"));
            }
            //info.ResearchPlanID
            string SavePathRelative = SaveWechatImage(info.Name, infoPlan);
            info.Name = SavePathRelative.Substring(SavePathRelative.LastIndexOf("/") + 1);
            info.PathRelative= SavePathRelative.Substring(0,SavePathRelative.LastIndexOf("/")+1);
            info.CreateDate = DateTime.Now;
            info.MineType = "images/jpg";
            info.CreateUserID = CurrentUser.ID;
            info.CreateUserName = CurrentUser.Name;
            info.TypeEnum = (int)SysEnum.ResearchPlanAttachmentType.课表;
            //info.Name = "课表" + Guid.NewGuid().ToString();
            info.Memo = string.Empty;
            info.ShowName = "课表" + infoPlan.ResearchPlanAttachmentInfo.Count() + 1;


            if (ResearchPlanAttachmentBLL.Create(info).ID>0)
            {
                //var datajson=infoPlan.ResearchPlanAttachmentInfo.Select(a => new { a.Name, FullPath = a.Name + a.PathRelative });
                return Json(new APIJson(0, "提交成功", GetImageJSON( infoPlan)));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        public JsonResult CreatePersonal(ResearchPlanAttachmentInfo info)
        {
            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(a => a.ID == info.ResearchPlanID).FirstOrDefault();
            var ifnoResearch = infoPlan.ResearchInfo.FirstOrDefault();
            if (infoPlan.DateBegin.Date < DateTime.Now.Date)
            {
                return Json(new APIJson(-1, "活动已过期"));
            }
            if (infoPlan.TypeEnum!=(int)SysEnum.ResearchPlanType.个人听课)
            {
                return Json(new APIJson(-1, "数据有误，不是个人听评课数据"));
            }
            //info.ResearchPlanID
            string SavePathRelative = SaveWechatImage(info.Name, infoPlan);
            info.Name = SavePathRelative.Substring(SavePathRelative.LastIndexOf("/") + 1);
            info.PathRelative = SavePathRelative.Substring(0, SavePathRelative.LastIndexOf("/") + 1);
            info.CreateDate = DateTime.Now;
            info.MineType = "images/jpg";
            info.CreateUserID = ifnoResearch.lectureUserID;
            info.CreateUserName = CurrentUser.Name;
            info.TypeEnum = (int)SysEnum.ResearchPlanAttachmentType.教案;
            //info.Name = "课表" + Guid.NewGuid().ToString();
            info.Memo = string.Empty;
            info.ShowName = "课表" + infoPlan.ResearchPlanAttachmentInfo.Count() + 1;


            if (ResearchPlanAttachmentBLL.Create(info).ID > 0)
            {
                //var datajson=infoPlan.ResearchPlanAttachmentInfo.Select(a => new { a.Name, FullPath = a.Name + a.PathRelative });
                return Json(new APIJson(0, "提交成功", GetImageJSON(infoPlan)));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }

        [HttpPost]
        public ActionResult Delete(int ID)
        {
            ResearchPlanAttachmentInfo info = ResearchPlanAttachmentBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null==info)
            {
                return Json(new APIJson("数据不存在"));
            }
            var infoPlan = info.ResearchPlanInfo;
            if (infoPlan.Status != (int)SysEnum.ResearchPlanStatus.未开始)
            {
                return Json(new APIJson(-1, "当前状态不能上传课表"));
            }

            try
            {
                System.IO.File.Delete(Server.MapPath(info.PathRelative + info.Name));
            }
            catch (Exception){}
            if (ResearchPlanAttachmentBLL.Delete(info))
            {

                return Json(new APIJson(0,"删除成功", GetImageJSON(infoPlan)));
            }
            return Json(new APIJson("删除失败，请重试"));
        }
        private const string ImageSavePathRelative = "/Content/file/ResearchPlan/";

        private string SaveWechatImage(string MediaID, ResearchPlanInfo infoPlan)
        {
            string RelativePath = ImageSavePathRelative + infoPlan.DateBegin.ToString("yyyyMM") + "/";
            string SaveMapPath = Server.MapPath(RelativePath);
            if (!Directory.Exists(SaveMapPath))
            {
                Directory.CreateDirectory(SaveMapPath);
            }
            string SaveName = DateTime.Now.ToFileTime().ToString() + ".jpg";

            WeiXin.APIClient.WechatService.WechatFile.GetMultimedia(WeiXin.APIClient.WechatService.GetAccessTonken(),
                MediaID, SaveMapPath, SaveName);
            return RelativePath + SaveName;
        }

        public Array GetImageJSON(ResearchPlanInfo infoPlan)
        {
            var result = infoPlan.ResearchPlanAttachmentInfo.Select(a => new { a.ID, FullPath = a.PathRelative+ a.Name }).ToList();
            return result.ToArray();
            //return Newtonsoft.Json.JsonConvert.SerializeObject());
        }


        /// <summary>
        /// 当前被评人员在组织调研中上传教案情况
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult ListLectureUser(int page = 1)
        {
            var list = ResearchPlanBLL.GetList(p => p.TypeEnum == (int)SysEnum.ResearchPlanType.组织调研);
            list = list.Where(a => a.DepartmentID == CurrentUser.DepartmentID);
            list = list.OrderByDescending(p => p.DateBegin).ThenByDescending(a => a.ID);
            IPagedList<ResearchPlanInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        /// <summary>
        /// 当前被评人员教案上传详细
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        public ActionResult DetailLectureUser(int PlanID)
        {
            var info = ResearchPlanBLL.GetList(a => a.ID == PlanID).FirstOrDefault();
            return View(info);
        }

    }
}
