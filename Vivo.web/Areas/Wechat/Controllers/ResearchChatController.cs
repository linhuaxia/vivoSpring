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
    public class ResearchChatController : BaseMPController
    {

        public ActionResult Index(ResearchInfo info)
        {
            var list = ResearchChatBLL.GetList(a =>
              a.ResearchPlanID == info.ResearchPlanID
              && a.GradeName == info.GradeName
              && a.ClassName == info.ClassName
              && a.LessionNumber==info.LessionNumber)
              .OrderBy(a=>a.ID).ToList()
            .Select(c => new {
                c.UserInfo.Name,
                CreateDate= c.CreateDate.ToString("HH:mm ss"),
                c.UserInfo.WechatHeadImg,
                c.Detail,
                IsSelf=(c.CreateUserID==CurrentUser.ID)

            }).ToList();
            return Json(new APIJson(0, "success", list));
        }

        public ActionResult Create(ResearchChatInfo info)
        {
            if (string.IsNullOrEmpty(info.Detail))
            {
                return Json(new APIJson(-1, "你总得输入点东西吧。。"));
            }
            info.CreateUserID = CurrentUser.ID;
            info.CreateDate = DateTime.Now;
            ResearchChatBLL.Create(info);
            if (info.ID>0)
            {
                return Json(new APIJson(0, "已发送"));
            }
            return Json(new APIJson(-1, "消息发送失败，请重试"));
        }
    }
}
