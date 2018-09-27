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
using System.Dynamic;
using System.Data.Entity;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class RepResearchNormalController : Vivo.web.Areas.MP.Controllers.BaseMPController
    {

        public ActionResult Index(int page = 1)
        {


            SysEnum.RepNormalType RepNormalType =SysEnum.Parse<SysEnum.RepNormalType>(Function.GetRequestString("RepNormalType"));
            var list = RepResearchNormalBLL.GetList(p => true);
            list = list.Where(a => a.TypeFlag == (int)RepNormalType);
            ViewBag.RepNormalType = RepNormalType;
            string Name = Function.GetRequestString("Name");
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");

            if (DateBegin>DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateBegin, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd>DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.DateEnd, DateEnd) >= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }
            switch (RepNormalType)
            {
                case SysEnum.RepNormalType.集体调研:
                    if (!PowerActionBLL.PowerCheck(PowerInfo.P_工作信息管理.PP常规.PPP集体调研.查看所有))
                    {
                        list = list.Where(a => a.CreateUserID == CurrentUser.ID);
                    }
                    break;
                case SysEnum.RepNormalType.一加三:
                    if (!PowerActionBLL.PowerCheck(PowerInfo.P_工作信息管理.PP常规.PPP一加三.查看所有))
                    {
                        list = list.Where(a => a.CreateUserID == CurrentUser.ID);
                    }
                    break;
                case SysEnum.RepNormalType.蹲点:
                    if (!PowerActionBLL.PowerCheck(PowerInfo.P_工作信息管理.PP常规.PPP蹲点.查看所有))
                    {
                        list = list.Where(a => a.CreateUserID == CurrentUser.ID);
                    }
                    break;
                case SysEnum.RepNormalType.教研组织:
                    if (!PowerActionBLL.PowerCheck(PowerInfo.P_工作信息管理.PP常规.PPP教研组织.查看所有))
                    {
                        list = list.Where(a => a.CreateUserID == CurrentUser.ID);
                    }
                    break;
                default:
                    break;
            }
            
            list = list.OrderByDescending(p => p.ID);
            IPagedList<RepResearchNormalInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        public ActionResult Create(SysEnum.RepNormalType RepNormalType)
        {
            ViewBag.listDepartment = DepartmentBLL.GetList(a => a.Enable).OrderBy(a => a.Name)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();
            ViewBag.RepNormalType= RepNormalType;
            return View("Create"+ (int)RepNormalType);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(RepResearchNormalInfo info)
        {
            if (info.DepartmentID<=0)
            {
                return Json(new APIJson(-1, "请选择学校地址"));
            }
            info.CreateUserID = CurrentUser.ID;
            info.CreateDate = DateTime.Now;
            if (null==info.Memo)
            {
                info.Memo = string.Empty;
            }
            info.Status = 0;
            if (info.DateBegin<=DicInfo.DateZone)
            {
                return Json(new APIJson(-1, "请选择正确的日期"));
            }
            info.DateEnd = info.DateBegin;
            if (info.TypeFlag <= 0)
            {
                return Json(new APIJson(-1, "数据格式有误"));
            }
            if (null==info.Title)
            {
                info.Title = string.Empty;
            }
            if (null==info.Detail)
            {
                info.Detail = string.Empty;
            }
            
            RepResearchNormalBLL.Create(info);
            if (info.ID > 0)
            {
                return Json(new APIJson(0, "添加成功"));
            }
            return Json(new APIJson(-1, "添加失败"));
        }

        public ActionResult Edit(int id)
        {
            RepResearchNormalInfo info = RepResearchNormalBLL.GetList(p => p.ID == id).FirstOrDefault();

            ViewBag.listDepartment = DepartmentBLL.GetList(a => a.Enable).OrderBy(a => a.Name)
                .Select(a => new SelectListItem() { Text = a.Name, Value = a.ID.ToString() }).ToList();


            return View("Edit"+info.TypeFlag,info);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(RepResearchNormalInfo info)
        {
            RepResearchNormalInfo infoExist = RepResearchNormalBLL.GetList(p => p.ID==info.ID).FirstOrDefault();
            if (infoExist == null)
            {
                return Json(new APIJson(-1, "数据不存在"));
            }
            infoExist.DepartmentID = info.DepartmentID;
            if (null!=info.Memo)
            {
                infoExist.Memo = info.Memo;
            }
            infoExist.DateBegin = info.DateBegin;
            if (info.Times>0)
            {
                infoExist.Times = info.Times;
            }
            if (info.Lessions>0)
            {
                infoExist.Lessions = info.Lessions;
            }
            if (info.Days > 0)
            {
                infoExist.Days = info.Days;
            }
            if (null!=info.Title)
            {
                infoExist.Title = info.Title;
            }
            if (null!=info.Detail)
            {
                infoExist.Detail = info.Detail;
            }
            if (RepResearchNormalBLL.Edit(infoExist))
            {
                return Json(new APIJson(0, "提交成功", info));
            }
            return Json(new APIJson(-1, "提交失败", info));
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            RepResearchNormalInfo info = RepResearchNormalBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (RepResearchNormalBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));
            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }

    }
}
