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
    public class ResearchPersonController : BaseMPController
    {
        public static readonly int PlanCategoryPersonalID = Function.ConverToInt(ConfigHelper.GetAppendSettingValue("PlanCategoryPersonalID"));

        public ActionResult Index(int page = 1)
        {
            infoHead.LeftURL = Url.Action("index","home");


            ViewBag.listUserLecture = UserBLL.GetList(a => true).ToList();

            var list = ResearchPlanBLL.GetList(p => p.TypeEnum == (int)SysEnum.ResearchPlanType.个人听课);
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP个人听课.PPP查看所有学校.KEY))
            {
                list = list.Where(a => CurrentUser.DepartmentID == a.DepartmentID);
            }
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP个人听课.PPP查看所有学科.KEY))
            {
                var listCurrentUserSubjectID = CurrentUser.SubjectInfo.Select(a => a.ID);
                list = list.Where(a => a.ResearchInfo.Any(r => listCurrentUserSubjectID.Contains(r.SubjectID)));
            }
            if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP个人听课.PPP仅查看个人被评.KEY) && CurrentUser.ID != DicInfo.AdminID)
            {
                list = list.Where(a => a.ResearchInfo.Any(r => r.lectureUserID == CurrentUser.ID));
            }
            if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP个人听课.PPP仅查看个人.KEY) && CurrentUser.ID != DicInfo.AdminID)
            {
                list = list.Where(a => a.ResearchInfo.Any(r => r.UserID == CurrentUser.ID));
            }

            list = list.OrderByDescending(p => p.DateBegin).ThenByDescending(a => a.ID);
            IPagedList<ResearchPlanInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }


        public ActionResult Create()
        {
            GetSelectList();

            ResearchPlanInfo infoPlan = new ResearchPlanInfo();
            infoPlan.DateBegin = infoPlan.DateEnd = DateTime.Now;

            return View(infoPlan);
        }


        private void GetSelectList()
        {
            ViewBag.listDepartment = DepartmentBLL.GetList(a => a.Enable).OrderBy(a => a.Name)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.listTemplate = EvalTemplateBLL.GetList(a => a.Enable).OrderByDescending(a => a.SortID).ThenByDescending(a => a.ID)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.listSubject =SubjectBLL.GetList(a=>a.Enable)// CurrentUser.SubjectInfo
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.listLessionNumber = Common.GetListOrderID();
            ViewBag.listGrade = Common.EnumToSelectListItem(typeof(SysEnum.LessionGrade));
            ViewBag.listArea = Common.EnumToSelectListItem(typeof(SysEnum.ResearchArea));
            List<SelectListItem> listClass = new List<SelectListItem>();
            for (int i = 1; i < 50; i++)
            {
                listClass.Add(new SelectListItem() { Text = i + "班", Value = i + "班" });
            }
            ViewBag.listClass = listClass;

        }

        [AllowAnonymous]
        public ActionResult Detail(int PlanID)
        {
            infoHead.LeftURL = Url.Action("index");

            var infoPlan = ResearchPlanBLL.GetList(a => a.ID == PlanID).FirstOrDefault();
            if (null == infoPlan || infoPlan.TypeEnum != (int)SysEnum.ResearchPlanType.个人听课)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "好像没有这样一个听课反馈窝" });
            }
            ResearchInfo info = infoPlan.ResearchInfo.FirstOrDefault();
            if (null == info)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "好像没有这样一个听课反馈窝" });
            }

            ViewBag.listResearchNote = info.ResearchNoteInfo.ToList()
                .Select(a => new {
                    a.ID,
                    a.Detail,
                    a.CreateDate,
                    ImageJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "image"),
                    AudioJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "audio")
                });
            ViewBag.listImagesResearchPlanJSON = new ResearchPlanAttachmentController().GetImageJSON(info.ResearchPlanInfo);
            return View(info);

        }
        [HttpPost]
        public ActionResult Create(ResearchPlanInfo info)
        {
            #region 计划对象ResearchPlanInfo
            info.Name = string.Format("{0}听评{1}", CurrentUser.Name, SysEnum.GetName(typeof(SysEnum.ResearchArea), info.AreaID));
            if (string.IsNullOrEmpty(info.Memo))
            {
                info.Memo = string.Empty;
            }
            if (info.DateBegin.Date < DateTime.Now.Date)
            {
                return Json(new APIJson("日期有误"));
            }
            if (info.TemplateID<=0)
            {
                return Json(new APIJson("请选择评价模板"));
            }
            info.CategoryID = PlanCategoryPersonalID;
            info.Address = string.Empty;
            info.DateEnd = info.DateBegin;
            info.TypeEnum = (int)SysEnum.ResearchPlanType.个人听课;
            info.CreateUserID = CurrentUser.ID;
            info.CreateDate = DateTime.Now;
            info.Status = (int)SysEnum.ResearchPlanStatus.进行中;

            info.SummaryDetail = string.Empty;
            info.SummaryUserName = string.Empty;
            #endregion

            #region 听评课对象ResearchInfo
            var infoResearch = new ResearchInfo();
            infoResearch.UserID = CurrentUser.ID;
            infoResearch.SubjectID = Function.GetRequestInt("SubjectID");
            infoResearch.lectureUserID = Function.GetRequestInt("lectureUserID", 0);
            infoResearch.Topic = Function.GetRequestString("Topic");
            infoResearch.LessionNumber = Function.GetRequestInt("LessionNumber");
            infoResearch.ClassName = Function.GetRequestString("ClassName");
            infoResearch.GradeName = Function.GetRequestString("GradeName");
            infoResearch.SubjectiveOpinion = string.Empty;
            infoResearch.NoteMemo = string.Empty;
            infoResearch.Status = (int)SysEnum.ResearchStatus.未评;
            infoResearch.ScoreTotal = 0;
            infoResearch.ScoreLever = (int)SysEnum.ResearchScoreLever.不合格;
            infoResearch.CreateDate = DateTime.Now;
            var infoExist = ResearchBLL.GetList(a =>
                DbFunctions.DiffDays(a.CreateDate, infoResearch.CreateDate) == 0
                && a.ResearchPlanID==infoResearch.ResearchPlanID
                && a.UserID == CurrentUser.ID
                && a.ClassName == infoResearch.ClassName
                && a.GradeName == infoResearch.GradeName
                && a.LessionNumber == infoResearch.LessionNumber
            ).FirstOrDefault();
            if (null != infoExist)
            {
                return Json(new APIJson(2, "您还在听评当前课程，将直接进入", new { PlanID = infoExist.ResearchPlanID, ResearchID = infoExist.ID }));
            }
            //infoResearch.ResearchPlanID
            //infoResearch.SubjectID
            if (string.IsNullOrEmpty(infoResearch.Topic))
            {
                return Json(new APIJson(-1, "请填写课题"));
            }
            if (string.IsNullOrEmpty(infoResearch.ClassName))
            {
                return Json(new APIJson(-1, "请填写年级"));
            }
            if (string.IsNullOrEmpty(infoResearch.GradeName))
            {
                return Json(new APIJson(-1, "请填写班级"));
            }
            if (infoResearch.SubjectID <= 0)
            {
                return Json(new APIJson(-1, "请选择学科"));
            }
            SubjectInfo infoSubject = SubjectBLL.GetList(a => a.ID == infoResearch.SubjectID).FirstOrDefault();
            if (null == infoSubject)
            {
                return Json(new APIJson(-1, "请选择学科"));
            }
            infoResearch.SubjectInfo = infoSubject;
            infoResearch.SubjectiveOpinion = string.Empty;
            infoResearch.NoteMemo = string.Empty;
            infoResearch.Status = (int)SysEnum.ResearchStatus.未评;
            infoResearch.ScoreTotal = 0;
            infoResearch.ScoreLever = (int)SysEnum.ResearchScoreLever.不合格;

            infoResearch.ResearchPlanInfo = info;
            info.ResearchInfo.Add(infoResearch);
            #endregion


            if (!CreatePConfirmDepartmentID(info))
            {
                return Json(new APIJson(-1, "请认真输放学校信息，对于系统未录入的学校，请务必认真输入全名"));
            }
            if (!CreatePConfirmlectureUserID(infoResearch))
            {
                return Json(new APIJson(-1, "请认真输放听课对象，对于系统未录入的老师，请务必认真填写其姓名"));
            }
            if (ResearchPlanBLL.Create(info).ID > 0)
            {
                return Json(new APIJson(0, "提交成功", new { PlanID = info.ID, ResearchID = infoResearch.ID }));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }

        public ActionResult EditPlan(int PlanID)
        {
            infoHead.LeftURL = Url.Action("index");

            GetSelectList();

            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(a => a.ID == PlanID).FirstOrDefault();
            infoPlan.DateBegin = infoPlan.DateEnd = DateTime.Now;

            return View(infoPlan);
        }

        [HttpPost]
        public ActionResult EditPlan(ResearchPlanInfo info)
        {
            ResearchPlanInfo infoExist = ResearchPlanBLL.GetList(a => a.ID == info.ID).FirstOrDefault();
            #region 计划对象ResearchPlanInfo
            if (string.IsNullOrEmpty(info.Memo))
            {
                info.Memo = string.Empty;
            }
            if (info.DateBegin.Date < DateTime.Now.Date)
            {
                return Json(new APIJson("日期有误"));
            }
            infoExist.Memo = info.Memo;
            infoExist.DateBegin = infoExist.DateEnd = info.DateBegin;

            #endregion

            #region 听评课对象ResearchInfo
            var infoResearch = infoExist.ResearchInfo.FirstOrDefault();
            //infoResearch.UserID = CurrentUser.ID;
            infoResearch.SubjectID = Function.GetRequestInt("SubjectID");
            infoResearch.lectureUserID = Function.GetRequestInt("lectureUserID", 0);
            infoResearch.Topic = Function.GetRequestString("Topic");
            infoResearch.LessionNumber = Function.GetRequestInt("LessionNumber");
            infoResearch.ClassName = Function.GetRequestString("ClassName");
            infoResearch.GradeName = Function.GetRequestString("GradeName");
          //  infoResearch.SubjectiveOpinion = string.Empty;
            infoResearch.NoteMemo = string.Empty;
          //  infoResearch.Status = (int)SysEnum.ResearchStatus.未评;
           // infoResearch.ScoreTotal = 0;
          //  infoResearch.ScoreLever = (int)SysEnum.ResearchScoreLever.不合格;
          //  infoResearch.CreateDate = DateTime.Now;
           
            if (string.IsNullOrEmpty(infoResearch.Topic))
            {
                return Json(new APIJson(-1, "请填写课题"));
            }
            if (string.IsNullOrEmpty(infoResearch.ClassName))
            {
                return Json(new APIJson(-1, "请填写年级"));
            }
            if (string.IsNullOrEmpty(infoResearch.GradeName))
            {
                return Json(new APIJson(-1, "请填写班级"));
            }
            if (infoResearch.SubjectID <= 0)
            {
                return Json(new APIJson(-1, "请选择学科"));
            }
            SubjectInfo infoSubject = SubjectBLL.GetList(a => a.ID == infoResearch.SubjectID).FirstOrDefault();
            if (null == infoSubject)
            {
                return Json(new APIJson(-1, "请选择学科"));
            }
            //infoResearch.SubjectInfo = infoSubject;
            //infoResearch.SubjectiveOpinion = string.Empty;
            //infoResearch.NoteMemo = string.Empty;
            //infoResearch.Status = (int)SysEnum.ResearchStatus.未评;
            //infoResearch.ScoreTotal = 0;
            //infoResearch.ScoreLever = (int)SysEnum.ResearchScoreLever.不合格;

            #endregion

            
            if (!CreatePConfirmlectureUserID(infoResearch))
            {
                return Json(new APIJson(-1, "请认真输入听课对象，对于系统未录入的老师，请务必认真填写其姓名"));
            }
            if (ResearchPlanBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功", new { info.ID }));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


        /// <summary>
        /// 检查听评对象用记是否存在，不存在则创建
        /// </summary>
        private bool CreatePConfirmlectureUserID(ResearchInfo infoResearch)
        {
            UserInfo infoUser = null;
            if (infoResearch.lectureUserID > 0)
            {
                infoUser = UserBLL.GetList(a =>
                a.ID == infoResearch.lectureUserID
                && a.DepartmentID == infoResearch.ResearchPlanInfo.DepartmentID).FirstOrDefault();
                if (null != infoUser)
                {
                    return true;
                }
            }

            infoUser = new UserInfo();
            infoUser.DepartmentID = infoResearch.ResearchPlanInfo.DepartmentID;
            infoUser.Name = Function.GetRequestString("TxtLectureUserID");
            infoUser.Code = infoUser.Name;
            infoUser.PassWord = Guid.NewGuid().ToString().Replace("-", "");
            infoUser.Email = infoUser.Tel = string.Empty;
            infoUser.CreateDate = infoUser.LastDate = DateTime.Now;
            infoUser.Enable = false;
            infoUser.LocationX = infoUser.LocationY = 0;
            infoUser.WechatOpenID = infoUser.WechatNameNick = infoUser.WechatHeadImg = infoUser.Sex = string.Empty;
            infoUser.IDCard = string.Empty;
            infoUser.TypeID = -1;
            infoUser.DefaultSubjectID = infoResearch.SubjectID;
            if (string.IsNullOrEmpty(infoUser.Name))
            {
                return false;
            }

            //再根据名称找一，如果就不要加啦
            var ExistItem = UserBLL.GetList(a => a.Name.StartsWith(infoUser.Name + "|外区")).FirstOrDefault();
            if (null != ExistItem)
            {
                infoResearch.lectureUserID = ExistItem.ID;
                return true;
            }


            //做insert前最后修正
            infoUser.Name = infoUser.Code = infoUser.Name
                + "|"
                + SysEnum.GetName(typeof(SysEnum.ResearchArea), infoResearch.ResearchPlanInfo.AreaID)
                + Guid.NewGuid().ToString().Replace("-", "");
            infoUser.SubjectInfo.Add(infoResearch.SubjectInfo);
            return UserBLL.Create(infoUser).ID > 0;

        }
        /// <summary>
        /// 检查听评对象用记是否存在，不存在则创建
        /// </summary>
        private bool CreatePConfirmDepartmentID(ResearchPlanInfo infoPlan)
        {
            DepartmentInfo infoDepartment = null;
            if (infoPlan.DepartmentID > 0)
            {
                infoDepartment = DepartmentBLL.GetList(a => a.ID == infoPlan.DepartmentID).FirstOrDefault();
                if (null != infoDepartment)
                {
                    infoPlan.DepartmentID = infoDepartment.ID;
                    infoPlan.DepartmentInfo = infoDepartment;
                    return true;
                }
            }
            infoDepartment = new DepartmentInfo();
            infoDepartment.Name = Function.GetRequestString("TxtDepartment");
            infoDepartment.ParentID = 0;
            infoDepartment.Enable = false;
            infoDepartment.OrderID = 0;
            infoDepartment.TypeEmun = infoPlan.AreaID;
            infoDepartment.Address = string.Empty;
            if (string.IsNullOrEmpty(infoDepartment.Name))
            {
                return false;
            }

            //再根据名称找一，如果就不要加啦
            var ExistItem = DepartmentBLL.GetList(a => a.Name == infoDepartment.Name && a.TypeEmun == infoPlan.AreaID).FirstOrDefault();
            if (null != ExistItem)
            {
                infoPlan.DepartmentID = ExistItem.ID;
                infoPlan.DepartmentInfo = ExistItem;
                return true;
            }

            if (DepartmentBLL.Create(infoDepartment).ID > 0)
            {
                infoPlan.DepartmentID = infoDepartment.ID;
                infoPlan.DepartmentInfo = infoDepartment;
            }
            return infoDepartment.ID > 0;

        }

        public ActionResult Edit(int ID)
        {
            ResearchInfo info = ResearchBLL.GetList(a => a.ID == ID).FirstOrDefault();
            if (null == info || info.ResearchPlanInfo.TypeEnum != (int)SysEnum.ResearchPlanType.个人听课)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "好像没有这样一个听课反馈窝" });
            }
            infoHead.LeftURL = Url.Action("Detail",new { PlanID =info.ResearchPlanID});

            ViewBag.listResearchNote = info.ResearchNoteInfo.ToList()
                .Select(a => new {
                    a.ID,
                    a.Detail,
                    a.CreateDate,
                    ImageJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "image"),
                    AudioJSON = ResearchNoteAttachmentBLL.GetImageJSON(a, "audio")
                });
            ViewBag.listImagesResearchPlanJSON = new ResearchPlanAttachmentController().GetImageJSON(info.ResearchPlanInfo);
            return View(info);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var infoPlan = ResearchPlanBLL.GetList(a => a.ResearchInfo.Any(r => r.ID == id)).FirstOrDefault();
            if (ResearchPlanBLL.Delete(infoPlan))
            {
                return Json(new APIJson(0, "删除成功"));

            }
            return Json(new APIJson(-1, "删除失败，请重试"));
        }


    }
}
