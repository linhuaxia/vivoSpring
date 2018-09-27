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
    public class ResearchPlanController : BaseMPController
    {
        [AllowAnonymous]
        public ActionResult Index(int page = 1)
        {
            infoHead.LeftURL = Url.Action("index", "home");
            var list = ResearchPlanBLL.GetList(p => p.TypeEnum == (int)SysEnum.ResearchPlanType.组织调研);
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.查看所有学校))
            {
                if (PowerActionBLL.PowerCheck(PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.仅查看个人))
                {
                    list = list.Where(a => a.ResearchPlanUserInfo.Select(rpu => rpu.UserID).Contains(CurrentUser.ID));
                }
                else
                {
                    list = list.Where(a => CurrentUser.DepartmentID == a.DepartmentID);
                }
            }


            list = list.OrderByDescending(p => p.DateBegin).ThenByDescending(a => a.ID);
            IPagedList<ResearchPlanInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            infoHead.LeftURL = Url.Action("index");
            GetSelectList();
            ViewBag.listPlanCategory = PlanCategoryBLL.GetList(a => a.Enable).OrderByDescending(a => a.ID)
              .ToList().Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });
            return View();
        }


        private void GetSelectList()
        {
            ViewBag.listDepartment = DepartmentBLL.GetList(a => a.Enable && a.TypeEmun == (int)SysEnum.DepartmentTypeEnum.区内学校)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.listTemplate = EvalTemplateBLL.GetList(a => a.Enable).OrderByDescending(a => a.SortID).ThenByDescending(a => a.ID)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
        }

        [HttpPost]
        public ActionResult Create(ResearchPlanInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson("请填写名称"));
            }
            if (info.DepartmentID <= 0)
            {
                return Json(new APIJson("请选择学校"));
            }

            if (info.TemplateID <= 0)
            {
                return Json(new APIJson("请选择模板"));
            }

            if (string.IsNullOrEmpty(info.Memo))
            {
                info.Memo = string.Empty;
            }
            if (info.DateBegin.Date < DateTime.Now.Date)
            {
                return Json(new APIJson("计划调研日期有误"));
            }

            info.Address = string.Empty;
            info.DateEnd = info.DateBegin;
            info.TypeEnum = (int)SysEnum.ResearchPlanType.组织调研;
            info.CreateUserID = CurrentUser.ID;
            info.CreateDate = DateTime.Now;
            info.Status = (int)SysEnum.ResearchPlanStatus.未开始;
            info.SummaryUserName = string.Empty;
            info.SummaryDetail = string.Empty;

            if (ResearchPlanBLL.Create(info).ID > 0)
            {
                return Json(new APIJson(0, "提交成功", new { info.ID }));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        public ActionResult Detail(int id)
        {
            infoHead.LeftURL = Url.Action("index");
            ResearchPlanInfo info = ResearchPlanBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }



        public ActionResult Edit(int id)
        {
            infoHead.LeftURL = Url.Action("Detail", new { id = id });
            GetSelectList();
            ResearchPlanInfo info = ResearchPlanBLL.GetList(p => p.ID == id).FirstOrDefault();
            ViewBag.listPlanCategory = PlanCategoryBLL.GetList(a => a.Enable || a.ID == info.CategoryID).OrderByDescending(a => a.ID)
               .ToList().Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() });
            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ResearchPlanInfo info)
        {
            ResearchPlanInfo infoExist = ResearchPlanBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson("请填写名称"));
            }
            if (info.DepartmentID <= 0)
            {
                return Json(new APIJson("请选择学校"));
            }
            if (string.IsNullOrEmpty(info.Memo))
            {
                info.Memo = string.Empty;
            }
            if (info.DateBegin.Date < DateTime.Now.Date && info.DateBegin != infoExist.DateBegin)
            {
                return Json(new APIJson("计划调研日期有误"));
            }

            infoExist.CategoryID = info.CategoryID;
            infoExist.Name = info.Name;
            infoExist.DepartmentID = info.DepartmentID;
            infoExist.TemplateID = info.TemplateID;
            infoExist.DateBegin = info.DateBegin;
            infoExist.Memo = info.Memo;
            //infoExist.typ


            if (ResearchPlanBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


        public ActionResult SelectUser(int ID)
        {
            var info = ResearchPlanBLL.GetList(a => a.ID == ID).FirstOrDefault();
            ViewBag.listMemberGroup = MemberGroupBLL.GetList(a => a.Enable).OrderByDescending(a => a.SortID).ThenByDescending(a => a.ID)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.listSubject = SubjectBLL.GetList(a => true).ToList().Where(a => info.DepartmentInfo.DepartmentTypeInfo.Contains(a.DepartmentTypeInfo))
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "人员选择";
            infoHead.LeftURL = Url == null ? "" : Url.Action("Detail", new { ID = ID });
            ViewBag.WechatHeaderInfo = infoHead;

            ViewBag.SelectedUserJSON = Newtonsoft.Json.JsonConvert.SerializeObject(info.ResearchPlanUserInfo.Select(u => new
            {
                u.ID,
                u.UserID,
                Name = u.UserInfo.Name,
                u.IsConfirmed,
                subjectName = string.Join(",", u.UserInfo.SubjectInfo.Select(s => s.Name)),
                DepartmentName = u.UserInfo.DepartmentInfo.Name
            }));
            ViewBag.info = info;

            int MemberGroupID = Function.GetRequestInt("MemberGroupID");
            int SubjectID = Function.GetRequestInt("SubjectID");
            string Name = Function.GetRequestString("Name");
            var listUser = UserBLL.GetList(a => a.Enable && a.RuleInfo.Any(r => r.PowerActionInfo.Any(p => p.NewID == PowerInfo.P_评课管理.PP组织调研.PPP听评课管理.添加)));
            if (MemberGroupID > 0)
            {
                listUser = listUser.Where(a => a.MemberGroupInfo.Any(mg => mg.ID == MemberGroupID));
                ViewBag.DdlMemberGroup = MemberGroupID;
            }
            if (SubjectID > 0)
            {
                listUser = listUser.Where(a => a.SubjectInfo.Any(s => s.ID == SubjectID));
                ViewBag.DdlSubject = SubjectID;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                listUser = listUser.Where(a => (a.Name.Contains(Name) || a.Tel.Contains(Name) || a.Code.Contains(Name)));
                ViewBag.TxtName = Name;
            }

            listUser = listUser.OrderBy(a => a.Name);
            return View(listUser);
        }
        [HttpPost]
        public ActionResult SelectUser(int UserID, bool IsAdd, int ResearchPlanID)
        {

            var info = ResearchPlanBLL.GetList(a => a.ID == ResearchPlanID).FirstOrDefault();
            var infoUser = UserBLL.GetList(a => a.ID == UserID).FirstOrDefault();
            var infoPlanUserExist = info.ResearchPlanUserInfo.FirstOrDefault(a => a.UserID == UserID);
            var result = true;
            if (null == infoUser)
            {
                return Json(new APIJson(-1, "用户不存在"));
            }
            if (!IsAdd)
            {
                if (null != infoPlanUserExist)
                {
                    result = ResearchPlanUserBLL.Delete(infoPlanUserExist);
                }
            }
            else
            {
                if (null == infoPlanUserExist)
                {
                    infoPlanUserExist = new ResearchPlanUserInfo();
                    infoPlanUserExist.ResearchPlanID = info.ID;
                    infoPlanUserExist.UserID = UserID;
                    infoPlanUserExist.DateCreate = DateTime.Now;
                    infoPlanUserExist.DateConfirm = DicInfo.DateZone;
                    infoPlanUserExist.IsConfirmed = false;
                    infoPlanUserExist.Memo = string.Empty;
                    infoPlanUserExist.SumRemark = string.Empty;
                    info.ResearchPlanUserInfo.Add(infoPlanUserExist);
                    result = ResearchPlanBLL.Edit(info);
                    new WeiXin.APIClient.WechatService.TemplateMsg().ResponseTemplateMsg(infoPlanUserExist, infoUser.WechatOpenID);
                }
            }
            if (result)
            {
                var SelectedUserJSON = info.ResearchPlanUserInfo.ToList().Select(u => new
                {
                    u.ID,
                    u.UserID,
                    Name = u.UserInfo.Name,
                    u.IsConfirmed,
                    subjectName = string.Join(",", u.UserInfo.SubjectInfo.Select(s => s.Name)),
                    DepartmentName = u.UserInfo.DepartmentInfo.Name

                });
                return Json(new APIJson(0, "处理成功", SelectedUserJSON));
            }
            return Json(new APIJson(-1, "处理失败，请重试"));
        }


        public ActionResult Delete(int id)
        {
            var info = ResearchPlanBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (ResearchPlanBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

        /// <summary>
        /// 通知学校有组织调研
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>

        public ActionResult PushMsg(int ID)
        {
            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(a => a.ID == ID).FirstOrDefault();
            ViewBag.listRule = RuleBLL.GetList(a => a.Enable);
            return View(infoPlan);
        }

        [HttpPost]
        public ActionResult PushMsg(int ID,string TimeStramp)
        {
            string RulesID = Function.GetRequestString("RulesID");
            WeiXin.APIClient.WechatService.TemplateMsg templateMsgHelper = new WeiXin.APIClient.WechatService.TemplateMsg();
            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(a => a.ID == ID).FirstOrDefault();
            List<string> listRuleStr = RulesID.Split(',').ToList();
            if (null == listRuleStr || listRuleStr.Count() == 0)
            {
                return Json(new APIJson(-1, "没有接收人员"));
            }
            List<int> listRuleID = new List<int>();
            foreach (var item in listRuleStr)
            {
                int RuleID = Convert.ToInt32(item);
                if (RuleID <= 0)
                {
                    continue;
                }

                listRuleID.Add(RuleID);
            }
            var listUserTarget = UserBLL.GetList(a => a.DepartmentID == infoPlan.DepartmentID && a.RuleInfo.Select(r => r.ID).Any(r => listRuleID.Contains(r)));
            int SuccessFlag = 0;
            foreach (var itemUser in listUserTarget)
            {
                if (string.IsNullOrEmpty(itemUser.WechatOpenID))
                {
                    continue;
                }
                bool SendResult = templateMsgHelper.ResponseTemplateMsg(infoPlan, itemUser.WechatOpenID);
                if (SendResult)
                {
                    SuccessFlag++;
                }
            }
            return Json(new APIJson(0, SuccessFlag + "人推送成功"));

        }
    }
}
