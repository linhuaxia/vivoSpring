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
    public class MemberGroupController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {


            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "用户群组管理";
            ViewBag.WechatHeaderInfo = infoHead;
            //  GetSelectList();


            var list = MemberGroupBLL.GetList(p => true);

            list = list.OrderByDescending(p => p.SortID).ThenByDescending(a => a.ID);
            IPagedList<MemberGroupInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create()
        {
            ViewBag.listSort = Common.GetListOrderID();
            return View();
        }
        [HttpPost]
        public ActionResult Create(MemberGroupInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json("请填写名称");
            }
            if (string.IsNullOrEmpty(info.Description))
            {
                info.Description = string.Empty;
            }
            if (MemberGroupBLL.Create(info).ID>0)
            {
                return Json(new APIJson(0, "提交成功", new { info.ID}));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }
        public ActionResult Detail(int id)
        {
            MemberGroupInfo info = MemberGroupBLL.GetList(p => p.ID == id).FirstOrDefault();
            return View(info);
        }


        public ActionResult Edit(int id)
        {
            MemberGroupInfo info = MemberGroupBLL.GetList(p => p.ID == id).FirstOrDefault();
            ViewBag.listSort = Common.GetListOrderID();
            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(MemberGroupInfo info)
        {
            MemberGroupInfo infoExist = MemberGroupBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json("请填写名称");
            }
            if (string.IsNullOrEmpty(info.Description))
            {
                info.Description = string.Empty;
            }
            infoExist.Name = info.Name;
            infoExist.Description = info.Description;
            infoExist.SortID = info.SortID;
            infoExist.Enable = info.Enable;

            if (MemberGroupBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


        public ActionResult SelectUser(int ID)
        {
           var info= MemberGroupBLL.GetList(a => a.ID == ID).FirstOrDefault();
            ViewBag.SelectedUserJSON = Newtonsoft.Json.JsonConvert.SerializeObject(info.UserInfo.Select(u=>new {
                u.ID,
                u.Name,
                subjectName =string.Join(",",u.SubjectInfo.Select(s=>s.Name)),
                DepartmentName = u.DepartmentInfo.Name
            }));
            ViewBag.info = info;
            var listUser = UserBLL.GetList(a => a.Enable).OrderBy(a => a.Name);
            return View(listUser);
        }
        [HttpPost]
        public ActionResult SelectUser(int UserID,bool IsAdd,int MemberGroupID)
        {
            var info = MemberGroupBLL.GetList(a => a.ID == MemberGroupID).FirstOrDefault();
            var infoUser = info.UserInfo.FirstOrDefault(a => a.ID == UserID);
            var result = true;
            if (!IsAdd)
            {
                if (null!=infoUser)
                {
                    info.UserInfo.Remove(infoUser);
                    result = true;
                }
            }
            else
            {
                if (null==infoUser)
                {
                    infoUser = UserBLL.GetList(a => a.ID == UserID).FirstOrDefault();
                    if (null==infoUser)
                    {
                        result = false;
                    }
                    else
                    {
                        info.UserInfo.Add(infoUser);
                        result = true;
                    }
                }
            }
            if (result)
            {
                result = MemberGroupBLL.Edit(info);
            }
            if (result)
            {
                var SelectedUserJSON = info.UserInfo.ToList().Select(u => new
                {
                    u.ID,
                    u.Name,
                    subjectName = string.Join(",", u.SubjectInfo.Select(s => s.Name))
                });
                return Json(new APIJson(0, "处理成功", SelectedUserJSON));
            }
            return Json(new APIJson(-1, "处理失败，请重试"));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var info = MemberGroupBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (MemberGroupBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

    }
}
