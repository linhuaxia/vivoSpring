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
using System.Data.Entity;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class ReportController : BaseMPController
    {
        [AllowAnonymous]
        public ActionResult ReportA(int PlanID)
        {
            infoHead.LeftURL = Url.Action("index", "Research", new { ID = PlanID, PlanID = PlanID });

            return new MP.Controllers.ReportController().ReportA(PlanID);
        }
        [AllowAnonymous]
        public ActionResult Report1(int PlanID)
        {
            infoHead.LeftURL = Url.Action("index", "Research", new { ID = PlanID, PlanID = PlanID });


            ResearchPlanInfo info = ResearchPlanBLL.GetList(a => a.ID == PlanID).FirstOrDefault();
            List<int> listDepartmentTypeID = info.DepartmentInfo.DepartmentTypeInfo.Select(a => a.ID).ToList();
            var listSubject = SubjectBLL.GetList(a => listDepartmentTypeID.Contains(a.DepartmentTypeID)).ToList();
            var listValuableSubjectResearch = info.ResearchInfo.Where(a => a.UserInfo.SubjectInfo.Select(u => u.ID).Contains(a.SubjectID));

            var result = listSubject.Select(subject => new
            {
                ID = subject.ID,
                Name = subject.Name,
                CountTeacher = listValuableSubjectResearch.Where(r => r.SubjectID == subject.ID).GroupBy(r => r.lectureUserID).Count(),
                CountItems = listValuableSubjectResearch.Where(r => r.SubjectID == subject.ID).Count()
            }).OrderByDescending(a => a.CountItems).ToList();

            result.Add(new
            {
                ID = 0,
                Name = "小计",
                CountTeacher = result.Sum(r => r.CountTeacher),
                CountItems = result.Sum(r => r.CountItems)
            });
            ViewBag.listResult = result;

            return View(info);

        }

        [AllowAnonymous]
        public ActionResult Report3(int PlanID, int SubjectID)
        {
            //infoHead.LeftURL = Url.Action("ReportA", new { PlanID = PlanID });
            infoHead.LeftURL = Url.Action("index", "Research", new { ID = PlanID, PlanID = PlanID });

            ResearchPlanInfo info = ResearchPlanBLL.GetList(a => a.ID == PlanID).FirstOrDefault();
            var infoSubject = SubjectBLL.GetList(a => a.ID == SubjectID).FirstOrDefault();
            ViewBag.infoSubject = infoSubject;
            foreach (var item in info.EvalTemplateInfo.TemplateSubjectSummaryInfo)
            {
                var infoItemSummary = info.ResearchPlanSubjectSummaryInfo.Where(s => s.SubjectID == SubjectID)
                    .SelectMany(s => s.ResearchPlanSubjectSummaryItemInfo).FirstOrDefault(i => i.TemplateSummaryID == item.ID);
                if (null!=infoItemSummary)
                {
                    ViewData["Detail" + item.ID] = infoItemSummary.Detail;
                }
            }
            return View(info);
        }


        public ActionResult Index()
        {
            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "汇总查询";
            infoHead.LeftURL = Url.Action("index", "home");
            ViewBag.WechatHeaderInfo = infoHead;
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetSubjectByDepartment(int DepartmentID)
        {
            DepartmentInfo info = DepartmentBLL.GetList(a => a.ID == DepartmentID).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "请选择学校"));
            }
            var listSubject = info.DepartmentTypeInfo
                .SelectMany(a => a.SubjectInfo)
                .Distinct();
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.查看所有学科))
            {
                var CurrentUserListSubjectID = CurrentUser.SubjectInfo.Select(a => a.ID);
                listSubject = listSubject.Where(a => CurrentUserListSubjectID.Contains(a.ID));
            }
            var result = listSubject.OrderBy(a => a.Name).Select(a => new
            {
                a.ID,
                a.Name
            });
            return Json(new APIJson(0, "", result));
        }

        [AllowAnonymous]
        public JsonResult GetLectureUser(int DepartmentID, int SubjectID)
        {
            DepartmentInfo info = DepartmentBLL.GetList(a => a.ID == DepartmentID).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "请选择学校"));
            }
            var list = UserBLL.GetList(a => a.DepartmentID == DepartmentID && a.SubjectInfo.Any(s => s.ID == SubjectID));
            if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.仅查看个人被评) && CurrentUser.Name != "admin")
            {
                list = list.Where(a => a.ID == CurrentUser.ID);
            }
            var result = list.OrderBy(a => a.Name)
                   .Select(a => new
                   {
                       a.ID,
                       a.Name
                   });
            return Json(new APIJson(0, "", result));

        }

        #region 跟MP中的完全一样


        public ActionResult ResearchS()
        {
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");
            int SubjectID = Function.GetRequestInt("SubjectID");
            string Name = Function.GetRequestString("Name");

            var listUser = UserBLL.GetList(a => a.Enable);
            var listResearch = ResearchBLL.GetList(a => true);

            if (SubjectID > 0)
            {
                listUser = listUser.Where(a => a.SubjectInfo.Select(s => s.ID).Contains(SubjectID));
                ViewBag.DdlSubject = SubjectID;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                listUser = listUser.Where(a => a.Name.Contains(Name) || a.WechatNameNick.Contains(Name));
                ViewBag.TxtName = Name;
            }
            var listUserFinal = listUser.ToList().Where(a => PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.KEY, a));
            if (DateBegin != DicInfo.DateZone)
            {
                listResearch = listResearch.Where(a => DbFunctions.DiffDays(a.ResearchPlanInfo.DateBegin, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd != DicInfo.DateZone)
            {
                listResearch = listResearch.Where(a => DbFunctions.DiffDays(a.ResearchPlanInfo.DateBegin, DateEnd) >= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }
            var templistUserFinalIDs = listUserFinal.Select(u => u.ID);
            listResearch = listResearch.Where(a => templistUserFinalIDs.Contains(a.UserID));

            ViewBag.listResearch = listResearch;
            ViewBag.listUserFinal = listUserFinal;


            return View();
        }




        [AllowAnonymous]
        public ActionResult ResearchS_Detail(int SubjectID, int UserID,
            SysEnum.ResearchPlanType researchPlanType, DateTime? DateBegin = null, DateTime? DateEnd = null)
        {

            ViewBag.SubjectInfo = SubjectBLL.GetList(a => a.ID == SubjectID).FirstOrDefault();
            ViewBag.UserInfo = UserBLL.GetList(a => a.ID == UserID).FirstOrDefault();
            ViewBag.researchPlanType = researchPlanType;

            var list = ResearchBLL.GetList(a => true);
            list = list.Where(a => a.SubjectID == SubjectID);
            list = list.Where(a => a.UserID == UserID);
            if (null != DateBegin)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.ResearchPlanInfo.DateBegin, DateBegin) <= 0);

            }
            if (null != DateEnd)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.ResearchPlanInfo.DateEnd, DateEnd) >= 0);

            }
            list = list.Where(a => a.ResearchPlanInfo.TypeEnum == (int)researchPlanType);
            return View(list);
        }


        /// <summary>
        /// 学校调研报表
        /// </summary>
        public ActionResult ResearchDepartment(int DepartmentID = 0, int EvalTemplateID = 0)
        {
            ViewBag.listEvalTempate = EvalTemplateBLL.GetList(a => true).OrderByDescending(a => a.ID).ToList()
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });
            var listDepartment = DepartmentBLL.GetList(a => true);
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.查看所有学校))
            {
                listDepartment = listDepartment.Where(a => a.ID == CurrentUser.DepartmentID);
                DepartmentID = CurrentUser.DepartmentID;
            }
            ViewBag.listDepartment = listDepartment.Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });

            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");

            if (EvalTemplateID <= 0)
            {
                EvalTemplateID = EvalTemplateBLL.GetList(a => a.Enable).OrderByDescending(a => a.ID).FirstOrDefault().ID;
            }
            ViewBag.infoEvalTemplate = EvalTemplateBLL.GetList(a => a.ID == EvalTemplateID).FirstOrDefault();
            ViewBag.DdlEvalTemplateID = EvalTemplateID;
            var list = ResearchPlanBLL.GetList(a => a.TypeEnum == (int)SysEnum.ResearchPlanType.组织调研);

            if (DepartmentID > 0)
            {
                list = list.Where(a => a.DepartmentID == DepartmentID);
                ViewBag.DdlDepartmentID = DepartmentID;
            }
            if (DateBegin > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateBegin, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateEnd, DateEnd) <= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }

            return View(list);
        }

        /// <summary>
        /// 学校教师调研报表
        /// </summary>
        public ActionResult ResearchLectureUser()
        {
            int DepartmentID = Function.GetRequestInt("DepartmentID");
            int SubjectID = Function.GetRequestInt("SubjectID");
            int LectureUserID = Function.GetRequestInt("LectureUserID");
            int EvalTemplateID = Function.GetRequestInt("EvalTemplateID");
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");

            if (EvalTemplateID <= 0)
            {
                EvalTemplateID = EvalTemplateBLL.GetList(a => a.Enable).OrderByDescending(a => a.ID).FirstOrDefault().ID;
            }
            ViewBag.infoEvalTemplate = EvalTemplateBLL.GetList(a => a.ID == EvalTemplateID).FirstOrDefault();
            ViewBag.DdlEvalTemplateID = EvalTemplateID;


            ViewBag.listEvalTempate = EvalTemplateBLL.GetList(a => true).OrderByDescending(a => a.ID).ToList()
                 .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });
            var listDepartment = DepartmentBLL.GetList(a => true);
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.查看所有学校))
            {
                listDepartment = listDepartment.Where(a => a.ID == CurrentUser.DepartmentID);
                DepartmentID = CurrentUser.DepartmentID;
            }
            ViewBag.listDepartment = listDepartment.Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });
            ViewBag.DdlDepartmentID = DepartmentID;
            ViewBag.DdlSubjectID = SubjectID;
            ViewBag.LectureUserID = LectureUserID;
            if (DateBegin > DicInfo.DateZone)
            {
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd > DicInfo.DateZone)
            {
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");

            }
            ViewBag.EvalTemplateID = EvalTemplateID;

            var listResearch = ResearchBLL.GetList(a => a.UserInfo.SubjectInfo.Any(s => s.ID == a.SubjectID));
            if (LectureUserID > 0)
            {
                listResearch = listResearch.Where(a => a.lectureUserID == LectureUserID);
                var infoLectureUser = UserBLL.GetList(a => a.ID == LectureUserID).FirstOrDefault();
                if (null != infoLectureUser)
                {
                    ViewBag.TxtLectureUserID = infoLectureUser.Name;
                    ViewBag.infoLectureUser = infoLectureUser;
                }
            }
            else
            {
                listResearch = listResearch.Where(a => false);
            }


            return View(listResearch);
        }



        #endregion

    }
}
