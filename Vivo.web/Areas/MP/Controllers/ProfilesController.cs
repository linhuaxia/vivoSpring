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

namespace Vivo.web.Areas.MP.Controllers
{
    public class ProfilesController : BaseMPUnFilterController
    {
        // GET: MP/ProfilesInfo
        public ActionResult Index(int page = 1)
        {
           
            return View();
        }

        public ActionResult EamilSetting()
        {
            ViewBag.SenderName = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件人显示名, true);
            ViewBag.SenderHostName = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮件主机, true);
            ViewBag.SenderEmailAddress = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱地址, true);
            ViewBag.SenderEmailPwd = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱密码, true);
            ViewBag.SenderEmailPort = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱端口, true);
            ViewBag.ReplyAddress = ProfilesBLL.Get(ProfilesInfo.EmailItem.回复地址, true);
            ViewBag.ReplyDisplayName = ProfilesBLL.Get(ProfilesInfo.EmailItem.回复时显示名, true);
            return View();
        }
        [HttpPost]
        public ActionResult EamilSetting(int TimeStamp)
        {
            ProfilesInfo infoSenderName = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件人显示名, true);
            ProfilesInfo infoSenderHostName = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮件主机, true);
            ProfilesInfo infoSenderEmailAddress = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱地址, true);
            ProfilesInfo infoSenderEmailPwd = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱密码, true);
            ProfilesInfo infoSenderEmailPort = ProfilesBLL.Get(ProfilesInfo.EmailItem.发件邮箱端口, true);
            ProfilesInfo infoReplyAddress = ProfilesBLL.Get(ProfilesInfo.EmailItem.回复地址, true);
            ProfilesInfo infoReplyDisplayName = ProfilesBLL.Get(ProfilesInfo.EmailItem.回复时显示名, true);

            infoSenderName.Value = Function.GetRequestString("SenderName");
            infoSenderHostName.Value = Function.GetRequestString("SenderHostName");
            infoSenderEmailAddress.Value = Function.GetRequestString("SenderEmailAddress");
            infoSenderEmailPwd.Value = Function.GetRequestString("SenderEmailPwd");
            infoSenderEmailPort.Value = Function.GetRequestString("SenderEmailPort");
            infoReplyAddress.Value = Function.GetRequestString("ReplyAddress");
            infoReplyDisplayName.Value = Function.GetRequestString("ReplyDisplayName");


            ProfilesBLL.Edit(infoSenderName);
            ProfilesBLL.Edit(infoSenderHostName);
            ProfilesBLL.Edit(infoSenderEmailAddress);
            ProfilesBLL.Edit(infoSenderEmailPwd);
            ProfilesBLL.Edit(infoSenderEmailPort);
            ProfilesBLL.Edit(infoReplyAddress);
            ProfilesBLL.Edit(infoReplyDisplayName);
            return Content(MVCScriptHelper.AlertRefresh("更新成功",null));
        }
        
        public ActionResult Edit()
        {
            ProfilesInfo ifnoa = ProfilesBLL.Get("手机购买时间有效小时数", true);
            ProfilesInfo infoAiQiYiRate = ProfilesBLL.Get("AiQiYiRate", true);
            ProfilesInfo infoEnableAgenterName = ProfilesBLL.Get("EnableAgenterName", true);
            ViewBag.ifnoa = ifnoa;
            ViewBag.infoAiQiYiRate = infoAiQiYiRate;
            ViewBag.infoEnableAgenterName = infoEnableAgenterName;
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(int TimeStamp)
        {
            ProfilesInfo ifnoa = ProfilesBLL.Get("手机购买时间有效小时数", true);
            ProfilesInfo infoAiQiYiRate = ProfilesBLL.Get("AiQiYiRate", true);
            ProfilesInfo infoEnableAgenterName = ProfilesBLL.Get("EnableAgenterName", true);
            ifnoa.Value = Function.GetRequestString("Txtinfoa");
            infoAiQiYiRate.Value = Function.GetRequestString("TxtinfoAiQiYiRate");
            infoEnableAgenterName.Value = Function.GetRequestString("TxtinfoEnableAgenterName");

            ProfilesBLL.Edit(ifnoa);
            ProfilesBLL.Edit(infoAiQiYiRate);
            ProfilesBLL.Edit(infoEnableAgenterName);
            return Content(MVCScriptHelper.AlertRefresh("更新成功", null));
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult InportSetting(int TimeStamp)
        {
            ProfilesInfo DefaultRuleIDWhenInport = ProfilesBLL.Get(ProfilesInfo.InportSetting.DefaultRuleIDWhenInport, true);
            DefaultRuleIDWhenInport.Value = Function.GetRequestString("DefaultRuleIDWhenInport");


            ProfilesBLL.Edit(DefaultRuleIDWhenInport);
            return Content(MVCScriptHelper.AlertRefresh("更新成功", null));
        }


        public ActionResult WechatSubscribeContent()
        {
            ProfilesInfo info= ProfilesBLL.Get(ProfilesInfo.Wechat.SubcribeContent, true);
            return View(info);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult WechatSubscribeContent(ProfilesInfo info)
        {

            ProfilesInfo infoExist = ProfilesBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            infoExist.Value = info.Value;
            ProfilesBLL.Edit(infoExist);
            return Content(MVCScriptHelper.AlertRefresh("更新成功", null));
        }


    }
}
