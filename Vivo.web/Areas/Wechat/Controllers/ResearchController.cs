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
    public class ResearchController : BaseMPController
    {
        [AllowAnonymous]
        public ActionResult index(int PlanID)
        {
            infoHead.LeftURL = null == Url ? "" : Url.Action("index", "ResearchPlan", new { GoAction = "index", GoController = "Research" });


            ResearchPlanInfo info = ResearchPlanBLL.GetList(p => p.ID == PlanID).FirstOrDefault();
            ViewBag.listSubject = info.DepartmentInfo.DepartmentTypeInfo.SelectMany(a => a.SubjectInfo)
                .Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();

            ViewBag.listLessionNumber = Common.GetListOrderID();
            ViewBag.listGrade = Common.EnumToSelectListItem(typeof(SysEnum.LessionGrade));
            List<SelectListItem> listClass = new List<SelectListItem>();
            for (int i = 1; i < 50; i++)
            {
                listClass.Add(new SelectListItem() { Text = i + "班", Value = i + "班" });
            }
            ViewBag.listClass = listClass;

            ViewBag.infoPlan = info;

            var listResearch = info.ResearchInfo.Where(a => true);
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.查看所有学科))
            {
                listResearch = listResearch.Where(a => a.UserID == CurrentUser.ID || CurrentUser.SubjectInfo.Select(s => s.ID).Contains(a.SubjectID));
            }
            if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.仅查看个人被评) && CurrentUser.Name != "admin")
            {
                listResearch = listResearch.Where(a => a.lectureUserID == CurrentUser.ID);
            }
            if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.仅查看个人) && CurrentUser.Name != "admin")
            {
                listResearch = listResearch.Where(a => a.UserID == CurrentUser.ID);
            }

            string GradeS = Function.GetRequestString("GradeS");
            string NameS = Function.GetRequestString("NameS");
            int SubjectID = Function.GetRequestInt("SubjectID");
            if (SubjectID > 0)
            {
                listResearch = listResearch.Where(a => a.SubjectID == SubjectID);
                ViewBag.DdlSubjectID = SubjectID;
            }
            if (!string.IsNullOrEmpty(GradeS))
            {
                listResearch = listResearch.Where(a => a.GradeName == GradeS);
                ViewBag.DdlGradeS = GradeS;
            }
            if (!string.IsNullOrEmpty(NameS))
            {
                var listlectureUser = UserBLL.GetList(a => a.DepartmentID == info.DepartmentID && a.Name.Contains(NameS));
                listResearch = listResearch.Where(a => listlectureUser.Select(u => u.ID).Contains(a.lectureUserID));
                ViewBag.TxtNameS = NameS;
            }
            listResearch = listResearch.OrderByDescending(a => a.ID);
            ViewBag.listResearch = listResearch;


            return View();
        }

        [HttpPost]
        public ActionResult Create(ResearchInfo info)
        {

            var infoExist = ResearchBLL.GetList(a =>
                a.ResearchPlanID == info.ResearchPlanID
                && a.UserID == CurrentUser.ID
                && a.ClassName == info.ClassName
                && a.GradeName == info.GradeName
                && a.LessionNumber == info.LessionNumber
            ).FirstOrDefault();
            if (null != infoExist)
            {
                return Json(new APIJson(2, "您还在听评当前课程，将直接进入", new { infoExist.ID }));
            }
            info.UserID = CurrentUser.ID;
            //info.ResearchPlanID
            //info.SubjectID
            if (string.IsNullOrEmpty(info.Topic))
            {
                return Json(new APIJson(-1, "请填写课题"));
            }
            if (string.IsNullOrEmpty(info.ClassName))
            {
                return Json(new APIJson(-1, "请填写年级"));
            }
            if (string.IsNullOrEmpty(info.GradeName))
            {
                return Json(new APIJson(-1, "请填写班级"));
            }
            if (info.SubjectID <= 0)
            {
                return Json(new APIJson(-1, "请选择学科"));
            }
            if (info.lectureUserID <= 0)
            {
                return Json(new APIJson(-1, "教师不存在，请输入教师姓名关键字，并点击提示结果"));

            }
            info.SubjectiveOpinion = string.Empty;
            info.NoteMemo = string.Empty;
            info.Status = (int)SysEnum.ResearchStatus.未评;
            info.ScoreTotal = 0;
            info.ScoreLever = (int)SysEnum.ResearchScoreLever.不合格;
            info.CreateDate = DateTime.Now;
            ResearchBLL.Create(info);
            if (info.ID > 0)
            {
                return Json(new APIJson(0, "新听评课事项创建成功", new { info.ID }));
            }
            return Json(new APIJson(-1, "创建失败，请重试"));

        }

        [AllowAnonymous]
        public ActionResult Detail(int ID)
        {

            ResearchInfo info = ResearchBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null == info || info.ResearchPlanInfo.TypeEnum != (int)SysEnum.ResearchPlanType.组织调研)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "好像没有这样一个听课反馈窝" });
            }
            string GoBack = Function.GetRequestString("GoBack");
            if (string.IsNullOrEmpty(GoBack))
            {
                GoBack = Url.Action("index", new { PlanID = info.ResearchPlanID });
            }
            infoHead.LeftURL = null == Url ? "" : GoBack;

            ViewBag.listResearchNote = info.ResearchNoteInfo.ToList()
                .Select(a => new
                {
                    a.ID,
                    a.Detail,
                    a.CreateDate,
                    ImageJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "image"),
                    AudioJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "audio")
                });
            return View(info);
        }

        public ActionResult Edit(int ID)
        {
            infoHead.LeftURL = Url.Action("Detail", new { ID = ID });

            ResearchInfo info = ResearchBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null == info || info.ResearchPlanInfo.TypeEnum != (int)SysEnum.ResearchPlanType.组织调研)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "好像没有这样一个听课反馈窝" });
            }
            ViewBag.listResearchNote = info.ResearchNoteInfo.ToList()
                .Select(a => new
                {
                    a.ID,
                    a.Detail,
                    a.CreateDate,
                    ImageJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "image"),
                    AudioJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "audio")
                });
            return View(info);
        }



        [HttpPost]
        public ActionResult Edit(ResearchInfo info)
        {
            bool IsConfirm = Function.GetRequestString("HidenIsConfirm") == "true";

            string EditProp = Function.GetRequestString("EditProp");///仅更新哪些字段  TemplateItem:评价项|SubjectiveOpinion主观评价|null 所有
            ResearchInfo infoExist = ResearchBLL.GetList(a => a.ID == info.ID).FirstOrDefault();
            if (null == infoExist || infoExist.UserID != CurrentUser.ID)
            {
                return Json(new APIJson(-1, "数据非法。这不是你的评课数据"));
            }
            if (EditProp == "TemplateItem" || string.IsNullOrEmpty(EditProp))
            {
                foreach (var item in infoExist.ResearchPlanInfo.EvalTemplateInfo.TemplateItemInfo)
                {
                    ResearchItemInfo infoItemExist = infoExist.ResearchItemInfo.FirstOrDefault(a => a.TemplateItemID == item.ID);
                    if (null == infoItemExist)
                    {
                        infoItemExist = new ResearchItemInfo();
                        infoItemExist.ResearchID = infoExist.ID;
                        infoItemExist.TemplateItemID = item.ID;
                        infoExist.ResearchItemInfo.Add(infoItemExist);
                    }
                    infoItemExist.ScoreValue = Function.GetRequestInt("TxtEvalTemplate" + item.ID, -1);
                    if (infoItemExist.ScoreValue < 0)
                    {
                        return Json(new APIJson(-1, string.Format("请完善评价项 {0}", item.Name)));
                    }
                    if (infoExist.ResearchPlanInfo.EvalTemplateInfo.TypeFlag==(int)SysEnum.TemplateTypeFlag.分值模板)
                    {
                        if (infoItemExist.ScoreValue > item.MaxScore)
                        {
                            return Json(new APIJson(-1, string.Format("评价项 {0}最高给分不能超过{1} ", item.Name, item.MaxScore)));
                        }
                    }
                    
                }
                infoExist.ScoreTotal = infoExist.ResearchItemInfo.Sum(a => a.ScoreValue);
                infoExist.ScoreLever =0;

            }
            if (EditProp == "SubjectiveOpinion" || string.IsNullOrEmpty(EditProp))
            {
                infoExist.SubjectiveOpinion = info.SubjectiveOpinion;
                infoExist.NoteMemo = info.NoteMemo;
                if (null == infoExist.SubjectiveOpinion)
                {
                    infoExist.SubjectiveOpinion = string.Empty;
                }
                if (null == infoExist.NoteMemo)
                {
                    infoExist.NoteMemo = string.Empty;
                }
            }


            if (IsConfirm)
            {
                infoExist.Status = (int)SysEnum.ResearchStatus.已确认;
                if (infoExist.ResearchPlanInfo.TypeEnum == (int)SysEnum.ResearchPlanType.个人听课)
                {
                    infoExist.ResearchPlanInfo.Status = (int)SysEnum.ResearchPlanStatus.已结束;
                }
            }
            else if (infoExist.Status != (int)SysEnum.ResearchStatus.已确认)
            {
                infoExist.Status = (int)SysEnum.ResearchStatus.待确认;
            }
            if (ResearchBLL.Edit(infoExist))
            {
                string SuccessMsg = "评价保存成功";
                if (infoExist.Status != (int)SysEnum.ResearchStatus.已确认)
                {
                    // SuccessMsg += "。你还需对你的评价项进行确认提交";
                }
                else
                {
                    SuccessMsg += "。你已进行确认完成了此项评课活动";
                }
                return Json(new APIJson(0, SuccessMsg));
            }
            else
            {
                return Json(new APIJson(-1, "提交失败了，请重试"));
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var info = ResearchBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (ResearchBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }


        /// <summary>
        /// 被听评记录
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ListLectureUser(int page = 1)
        {

            var list = ResearchBLL.GetList(a => a.lectureUserID == CurrentUser.ID);
            list = list.OrderByDescending(a => a.ResearchPlanID);
            var result = list.ToPagedList(page, PageSize);
            return View(result);
        }

    }
}
