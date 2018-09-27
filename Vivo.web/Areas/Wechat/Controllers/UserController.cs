using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Vivo.Model;
using Vivo.IBLL;
using Vivo.BLLFactory;
using Tool;
using Vivo.web.Areas.Wechat.Models;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class UserController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {
            WechatHeaderInfo infoHead = new WechatHeaderInfo();
            infoHead.HeadText = "系统帐户管理";
            infoHead.LeftURL = Url.Action("index", "Home");
            ViewBag.WechatHeaderInfo = infoHead;

            GetSelectList();
            int RuleID = Function.GetRequestInt("RuleID");
            int MemberGroupID = Function.GetRequestInt("MemberGroupID");
            int DepartmentID = Function.GetRequestInt("DepartmentID");
            string Name = Function.GetRequestString("Name");

            var list = UserBLL.GetList(p => true);


            if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看所有帐户))
            {
                DepartmentID = CurrentUser.DepartmentID;
                if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看当前单位帐户))
                {
                    list = list.Where(a => a.ID == CurrentUser.ID);
                }
            }


            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(p => p.Name.Contains(Name));
                ViewBag.TxtName = Name;
            }
            if (RuleID > 0)
            {
                list = list.Where(p => p.RuleInfo.Any(r => r.ID == RuleID));
                ViewBag.DdlRuleID = RuleID;
            }
            if (MemberGroupID > 0)
            {
                list = list.Where(p => p.MemberGroupInfo.Any(M => M.ID == MemberGroupID));
                ViewBag.DdlMemberGroup = MemberGroupID;
            }
            if (DepartmentID > 0)
            {
                list = list.Where(p => p.DepartmentID == DepartmentID);
                ViewBag.DdlDepartment = DepartmentID;
            }

            list = list.OrderByDescending(p => p.Enable)
                .ThenByDescending(p => p.DepartmentID)
                .ThenByDescending(p => p.ID);
            var result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        private void GetSelectList()
        {
            ViewBag.listRule = RuleBLL.GetList(a => a.Enable).Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();
            ViewBag.listMemberGroup = MemberGroupBLL.GetList(a => a.Enable).Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();
            IEnumerable<SelectListItem> listDepartmentID = DepartmentBLL.GetList(p => p.Enable == true).Select(p => new SelectListItem { Text = p.Name, Value = p.ID.ToString() }).ToList();
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看所有帐户))
            {
                listDepartmentID = listDepartmentID.Where(a => a.Value == CurrentUser.ToString());
            }
            ViewBag.listDepartment = listDepartmentID;

        }

        public ActionResult Detail(int id)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson("参数有误"));
            }
            return View(info);
        }
        public ActionResult Create()
        {
            GetSelectList();
            return View();
        }
        [AllowAnonymous]
        public ActionResult OutCreate()
        {
            GetSelectList();
            return View();
        }
        public ActionResult Edit(int id)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return RedirectToAction("E404","Error",new { Msg = "参数有误" });
            }

            GetSelectList();
            ViewBag.listSubject = SubjectBLL.GetList(a => a.Enable).ToList().Where(a=>info.DepartmentInfo.DepartmentTypeInfo.Any(dt=>dt.ID== a.DepartmentTypeID))
                .Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();
            ViewBag.listRuleJSON = Newtonsoft.Json.JsonConvert.SerializeObject(info.RuleInfo.Select(a => new { a.ID, a.Name }));
            ViewBag.listSubjectJSON = Newtonsoft.Json.JsonConvert.SerializeObject(info.SubjectInfo.Select(a => new { a.ID, a.Name }));
            return View(info);
        }


        [AllowAnonymous]
        public ActionResult CurrentDetail()
        {
            UserInfo info = UserBLL.GetCurrent();
            if (null == info)
            {
                return Json(new APIJson("参数有误"));
            }
            info = UserBLL.GetList(p => p.ID == info.ID).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson("参数有误"));
            }
            ViewBag.WechatHeaderInfo = new WechatHeaderInfo() { HeadText = "登录信息" };
            return View(info);
        }

        [AllowAnonymous]
        public ActionResult APIQueue(int DepartmentID,int SubjectID=0)
        {
           var list= UserBLL.GetList(a => a.DepartmentID == DepartmentID);
            if (SubjectID>0)
            {
                list = list.Where(a=>a.SubjectInfo.Any(s=>s.ID==SubjectID));
            }
            var result = list.ToList().Select(a => new { a.ID, Name = a.Name +"/"+a.Tel }).ToList();
            return Json(new APIJson(0,"",result), JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult APIQueueAutoComplete(string q)
        {
            int DepartmentID = Function.GetRequestInt("DepartmentID");
            int SubjectID = Function.GetRequestInt("SubjectID");
            var list = UserBLL.GetList(a => a.DepartmentID == DepartmentID && a.SubjectInfo.Any(s=>s.ID==SubjectID));
            list = list.Where(a => a.Name.Contains(q) || a.Code.Contains(q) || a.Code.Contains(q) || a.Tel.Contains(q));
            var result = list.ToList().Select(a => new { a.ID, Name = a.Name + string.Join(",", a.SubjectInfo.Select(s => s.Name)) }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pwd(int ID)
        {
            UserInfo info = UserBLL.GetList(a => a.ID == ID).FirstOrDefault();
            return View(info);
        }


    }
}