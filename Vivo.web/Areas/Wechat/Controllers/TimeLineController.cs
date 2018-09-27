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
    public class TimeLineController : BaseMPController
    {

        public ActionResult Index()
        {
            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "收获反思";
            infoHead.LeftURL = Url.Action("index", "home");
            ViewBag.WechatHeaderInfo = infoHead;
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetListData(int TypeFlag, int page = 1)
        {
            
            var list = TimeLineBLL.GetList(p => true);
            if (TypeFlag == 1)
            {
                //var ListCurrentSubject = CurrentUser.SubjectInfo.Select(s => s.ID).ToList();
                //list = list.Where(a => ListCurrentSubject.Contains(a.ResearchInfo.SubjectID));
                list = list.Where(a=>a.UserInfo.Select(u=>u.ID).Contains(CurrentUser.ID));
            }
            else if (TypeFlag == 2)
            {
                list = list.Where(a => a.ResearchInfo.UserID == CurrentUser.ID);
            }

            list = list.OrderByDescending(p => p.ID);
            IPagedList<TimeLineInfo> result = list.ToPagedList(page, 5);
            var jsonResult = result.Select(a => new
            {
                a.ID,
                a.ResearchInfo.ResearchPlanID,
                TypeEnum = a.ResearchInfo.ResearchPlanInfo.TypeEnum == (int)SysEnum.ResearchPlanType.个人听课 ? "ResearchPerson" : "Research",
                a.ResearchInfo.Topic,
                DepartmentName = a.ResearchInfo.ResearchPlanInfo.DepartmentInfo.Name,
                SubjectName = a.ResearchInfo.SubjectInfo.Name,
                a.ResearchInfo.UserInfo.WechatHeadImg,
                a.ResearchInfo.UserInfo.Name,
                UserID = a.ResearchInfo.UserInfo.ID,
                a.Detail,
                a.ResearchID,
                CreateDate = a.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                listAttachment = a.ResearchInfo.ResearchNoteInfo.SelectMany(researchNote => researchNote.ResearchNoteAttachmentInfo)
                .Where(at => at.MineType == "image").Select(at => new {
                    at.ID,
                    at.PathRelative,
                    at.Name
                }),
                listCommon = a.TimeLineCommonInfo.Select(c => new {
                    c.ID,
                    c.UserInfo.Name,
                    c.IsCommon,
                    c.Detail
                }),
                listImage = a.ResearchInfo.ResearchNoteInfo.SelectMany(rn => rn.ResearchNoteAttachmentInfo).Where(rna => rna.MineType == "image")
                    .Select(rna=>new {
                        rna.ID,
                        src=rna.PathRelative+rna.Name
                    })
            });
            return Json(jsonResult, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create(int ResearchID)
        {
            TimeLineInfo info = new TimeLineInfo();
            info.ResearchID = ResearchID;
            return View(info);
        }


        [HttpPost]
        public ActionResult Create(TimeLineInfo info)
        {
            if (string.IsNullOrEmpty(info.Detail))
            {
                return Json(new APIJson(-1, "总得写点东西吧"));
            }
            ResearchInfo infoResearch = ResearchBLL.GetList(a => a.ID == info.ResearchID).FirstOrDefault();
            if (null==infoResearch||infoResearch.UserID!=CurrentUser.ID)
            {
                return Json(new APIJson(-1, "提交失败，数据有误"));
            }
            info.CreateDate = DateTime.Now;
            string checkboxUser = Function.GetRequestString("checkboxUser");
            info.UserInfo = new List<UserInfo>();
            foreach (var item in checkboxUser.Split(','))
            {
                int ID = Function.ConverToInt(item);
                if (ID<=0)
                {
                    continue;
                }
                UserInfo u = UserBLL.GetList(a => a.ID == ID).FirstOrDefault();
                if (null!=u && !info.UserInfo.Any(a=>a.ID==ID))
                {
                    info.UserInfo.Add(u);
                }

            }
            if (TimeLineBLL.Create(info).ID > 0)
            {
                return Json(new APIJson(0, "提交成功，分享到微信中时，你现在可以打开右上角菜单进行分享了", new { info.ID }));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


    }
}
