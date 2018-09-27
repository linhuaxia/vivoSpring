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
    public class ResearchPlanUserController : BaseMPController
    {
        [AllowAnonymous]
        public ActionResult Confirm(int PlanID)
        {
            ResearchPlanInfo info = ResearchPlanBLL.GetList(p => p.ID == PlanID).FirstOrDefault();
            if (info.DateBegin < DateTime.Now.Date)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "计划已过期" });
            }
            var infoUser = info.ResearchPlanUserInfo.First(a => a.UserID == CurrentUser.ID);
            if (null == infoUser)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "当前计划并没有邀请到您哦", Detail = "管理员可能已将您去除邀请" });
            }

            return View(infoUser);
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Confirm(ResearchPlanUserInfo info)
        {
            ResearchPlanUserInfo infoExist = ResearchPlanUserBLL.GetList(p => p.ID == info.ID).FirstOrDefault();

            infoExist.IsConfirmed = true;
            infoExist.DateConfirm = DateTime.Now;
            if (ResearchPlanUserBLL.Edit(infoExist))
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
            ViewBag.listSubject = SubjectBLL.GetList(a => info.DepartmentInfo.DepartmentTypeInfo.Contains(a.DepartmentTypeInfo))
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "人员选择";
            infoHead.LeftURL = Url.Action("Detail", new { ID = ID });
            ViewBag.WechatHeaderInfo = infoHead;

            ViewBag.SelectedUserJSON = Newtonsoft.Json.JsonConvert.SerializeObject(info.ResearchPlanUserInfo.Select(u => new
            {
                u.ID,
                u.UserID,
                Name = u.UserInfo.Name,
                subjectName = string.Join(",", u.UserInfo.SubjectInfo.Select(s => s.Name))
            }));
            ViewBag.info = info;

            int MemberGroupID = Function.GetRequestInt("MemberGroupID");
            int SubjectID = Function.GetRequestInt("SubjectID");
            string Name = Function.GetRequestString("Name");
            var listUser = UserBLL.GetList(a => a.Enable);
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
                    subjectName = string.Join(",", u.UserInfo.SubjectInfo.Select(s => s.Name))
                });
                return Json(new APIJson(0, "处理成功", SelectedUserJSON));
            }
            return Json(new APIJson(-1, "处理失败，请重试"));
        }


    }
}
