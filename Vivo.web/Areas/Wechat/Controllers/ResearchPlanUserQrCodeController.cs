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
    public class ResearchPlanUserQrCodeController : BaseWechatUnUserController
    {
        public ActionResult Join(int ID)
        {
            ViewBag.ProfilesBLL = ProfilesBLL;

            ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(p => p.ID == ID).FirstOrDefault();
            if (infoPlan.DateBegin < DateTime.Now.Date)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "计划已过期" });
            }
            if (null!=UserBLL.GetCurrent())
            {
                return RedirectToAction("index", "Research", new { ID = ID, PlanID=ID });
            }
            if (null == infoWechatUserReturn)
            {
                return RedirectToAction("Msg", "CommPage", new { Title = "系统无法获取到您的个人信息" });
            }
            var infoUser = UserBLL.GetList(a => a.WechatOpenID == infoWechatUserReturn.openid).FirstOrDefault();
            if (null != infoUser && infoPlan.ResearchPlanUserInfo.Any(a => a.UserID == infoUser.ID))
            {
                return RedirectToAction("Detail", "ResearchPlan", new { ID = infoPlan.ID });
            }

            ViewBag.infoPlan = infoPlan;
            ViewBag.infoWechatUserReturn = infoWechatUserReturn;
            return View(infoUser);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Join(UserInfo info)
        {
            string log = string.Empty;
            try
            {

                int PlanID = Function.GetRequestInt("PlanID");
                log += "\n PlanID=" + PlanID;
                string infoWechatUserReturnOpenID = Function.GetRequestString("infoWechatUserReturnOpenID");
                log += "\n infoWechatUserReturnOpenID=" + infoWechatUserReturnOpenID;
                ResearchPlanInfo infoPlan = ResearchPlanBLL.GetList(p => p.ID == PlanID).FirstOrDefault();
                log += "\n infoPlan=" + infoPlan;
                UserInfo infoUser = null;
                if (info.ID > 0)
                {
                    log += "\n info.ID > 0=";
                    infoUser = UserBLL.GetList(a => a.ID == info.ID).FirstOrDefault();
                }
                if (null == infoUser)
                {
                    log += "\n info.ID < 0=";
                    infoUser = UserBLL.GetList(a => a.WechatOpenID == infoWechatUserReturnOpenID).FirstOrDefault();
                }
                log += "\n infoUser=" + infoUser;
                if (null == infoUser)
                {
                    log += "\n infoUser is null";
                    WechatUserReturnInfo infoWechatUserReturn = WeiXin.APIClient.WechatService.WechatUser.GetWechatUserReturnInfo(infoWechatUserReturnOpenID);
                    if (null == infoWechatUserReturn)
                    {
                        return Json(new APIJson(-1, "无法获取用户信息"));
                    }
                    log += "\n infoWechatUserReturn="+ infoWechatUserReturn;
                    infoUser = new UserInfo();
                    //infoUser.DepartmentID 从config中读到表单hiden了里
                    //infoUser.Name
                    if (string.IsNullOrEmpty(info.Name) || info.Name.Length > 50)
                    {
                        return Json(new APIJson(-1, "请输入您的姓名"));
                    }
                    if (UserBLL.GetList(a => true).Any(a => a.Name == infoUser.Name))
                    {
                        return Json(new APIJson(-1, "系统里居然有人跟你同名了，你换一个或加个数字后缀吧"));
                    }
                    var infoUserExistName = UserBLL.GetList(a => a.Name == info.Name).FirstOrDefault();
                    if (null!=infoUserExistName)
                    {
                        return Json(new APIJson(-1, "系统已存在当前用户名，请更换"));
                    }
                    infoUser.DepartmentID = info.DepartmentID;
                    infoUser.Name = info.Name;
                    infoUser.Code = infoUser.Name;
                    infoUser.PassWord = string.Empty;
                    //infoUser.Email
                    if (null == infoUser.Email)
                    {
                        infoUser.Email = string.Empty;
                    }
                    infoUser.Email = info.Email;
                    infoUser.Tel = string.Empty;
                    infoUser.CreateDate = DateTime.Now;
                    infoUser.LastDate = DateTime.Now;
                    infoUser.Enable = true;
                    infoUser.LocationX = infoUser.LocationY = 0;
                    infoUser.WechatOpenID = infoWechatUserReturn.openid;
                    infoUser.WechatNameNick = infoWechatUserReturn.nickname;
                    infoUser.WechatHeadImg = infoWechatUserReturn.headimgurl;
                    infoUser.Sex = infoWechatUserReturn.sex.ToString();
                    infoUser.IDCard = string.Empty;
                    infoUser.TypeID = -1;
                    infoUser.DefaultSubjectID = 0;

                    log += "\n 准备完后infoUser=" + Newtonsoft.Json.JsonConvert.SerializeObject(infoUser);

                    int RuleDefaultID = Function.ConverToInt(ProfilesBLL.GetValue(ProfilesInfo.OutSideUserSetting.RuleID));
                    RuleInfo infoRule = RuleBLL.GetList(a => a.ID == RuleDefaultID).FirstOrDefault();
                    if (null != infoRule)
                    {
                        infoUser.RuleInfo.Add(infoRule);
                    }
                    infoUser = UserBLL.Create(infoUser);
                    log += "\n 创建完了" ;

                }

                ResearchPlanUserInfo infoPlanUser = new ResearchPlanUserInfo();
                infoPlanUser.ResearchPlanID = info.ID;
                infoPlanUser.UserID = infoUser.ID;
                infoPlanUser.DateCreate = DateTime.Now;
                infoPlanUser.DateConfirm = DicInfo.DateZone;
                infoPlanUser.IsConfirmed = true;
                infoPlanUser.Memo = "二维码邀请";
                infoPlanUser.SumRemark = string.Empty;
                infoPlan.ResearchPlanUserInfo.Add(infoPlanUser);
                var result = ResearchPlanBLL.Edit(infoPlan);
                if (result)
                {
                    return Json(new APIJson(0, "恭喜您，成功加入本次听评课计划"));
                }
                else
                {
                    return Json(new APIJson(-1, "加入失败了，请重试"));
                }
            }
            catch (Exception ex)
            {
                var ex2 = (System.Data.Entity.Infrastructure.DbUpdateException)ex;

                var ErrorMsg = log
                    + "Ex========" + ex.Message 
                    + "\n ex2.InnerException.ObjectToJSON();" + ex2.InnerException.ObjectToJSON();
                return Json(new APIJson(-1, ErrorMsg));
                throw;
            }
        }




    }
}
