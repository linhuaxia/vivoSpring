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
using System.Data;
using System.IO;
using System.Data.Entity;

namespace Vivo.web.Areas.MP.Controllers
{
    public class PrizeResultController : BaseMPUnFilterController
    {

        public ActionResult Index(int page = 1)
        {
            ViewBag.listPrizeType = Common.EnumToSelectListItem(typeof(SysEnum.PrizeType));
            ViewBag.listPlan = PlanBLL.GetList(a => true).OrderBy(a => a.DateBegin).ToList()
                .Select(a => new SelectListItem() { Text = a.AgenterName + "|" + a.MarketLevel + "|" + a.DateBegin.ToString("MM-dd HH:mm"), Value = a.ID.ToString() });

            IQueryable<PrizeResultInfo> list = GetListData();
            IPagedList<PrizeResultInfo> result = list.ToPagedList(page, PageSize);
            return View(result);
        }

        private IQueryable<PrizeResultInfo> GetListData()
        {
            var list = PrizeResultBLL.GetList(p => true);

            int PlanID = Function.GetRequestInt("PlanID");
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");
            string Name = Function.GetRequestString("Name");
            int PrizeType = Function.GetRequestInt("PrizeType");
            if (PlanID > 0)
            {
                list = list.Where(a => a.PlanID == PlanID);
                ViewBag.DdlPlanID = PlanID;
            }
            if (DateBegin > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.CreateDate, DateBegin) <= 0);
                ViewBag.TxtDateBegin = DateBegin.ToString("yyyy-MM-dd");
            }
            if (DateEnd > DicInfo.DateZone)
            {
                list = list.Where(a => DbFunctions.DiffDays(a.CreateDate, DateEnd) >= 0);
                ViewBag.TxtDateEnd = DateEnd.ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name) || a.Tel.Contains(Name));
                ViewBag.TxtName = Name;
            }
            if (PrizeType > 0)
            {
                if (PrizeType == (int)SysEnum.PrizeType.一等奖)
                {
                    string tempp = ((int)SysEnum.PrizeType.一等奖).ToString();
                    list = list.Where(a => a.Result == tempp);
                }
                else if (PrizeType == (int)SysEnum.PrizeType.二等奖)
                {
                    string tempp = ((int)SysEnum.PrizeType.二等奖).ToString();
                    list = list.Where(a => a.Result == tempp);
                }
                else if (PrizeType == (int)SysEnum.PrizeType.爱奇艺会员)
                {
                    list = list.Where(a => a.Result.Contains("-"));
                }
                else if (PrizeType == (int)SysEnum.PrizeType.没中奖)
                {
                    list = list.Where(a => a.Result == "");
                }
                ViewBag.DdlPrizeType = PrizeType;
            }
            list = list.OrderByDescending(p => p.ID);
            return list;
        }

        public ActionResult Export()
        {
            IQueryable<PrizeResultInfo> list = GetListData();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("OpenID");
            dt.Columns.Add("姓名");
            dt.Columns.Add("电话");
            dt.Columns.Add("SN号");
            dt.Columns.Add("抽奖时间");
            dt.Columns.Add("抽奖结果");
            dt.Columns.Add("限定活动");
            foreach (var item in list)
            {

                DataRow dr = dt.NewRow();
                dr["ID"] = item.ID;
                dr["OpenID"] = item.OpenID;
                dr["姓名"] = item.Name;
                dr["电话"] = item.Tel;
                dr["SN号"] = item.SnNumber;
                dr["抽奖时间"] = item.CreateDate;
                dr["抽奖结果"] = item.Result;
                dr["限定活动"] = string.Format("{0}|{1}|{2}到{3}",
                     item.PlanInfo.AgenterName,
                     item.PlanInfo.MarketLevel,
                     item.PlanInfo.DateBegin.ToString("MM-dd HH"),
                     item.PlanInfo.DateEnd.ToString("MM-dd HH"));
                dt.Rows.Add(dr);
            }
            string FilePathRelative = "/Content/Export/PrizeResult/";
            string FilePathAbs = Server.MapPath(FilePathRelative);
            string FileName = DateTime.Now.ToFileTime().ToString()+".xlsx";
            if (!Directory.Exists(FilePathAbs))
            {
                Directory.CreateDirectory(FilePathAbs);
            }
            NPOIHelper.Export(dt, FilePathAbs + FileName);
            return Redirect(FilePathRelative + FileName);
        }


    }
}
