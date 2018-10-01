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
            IQueryable<PrizeResultInfo> list = GetListData();
            IPagedList<PrizeResultInfo> result = list.ToPagedList(page, 20);
            return View(result);
        }

        private IQueryable<PrizeResultInfo> GetListData()
        {
            var list = PrizeResultBLL.GetList(p => true);
            string RS = Function.GetRequestString("RS");
            DateTime DateBegin = Function.GetRequestDateTime("DateBegin");
            DateTime DateEnd = Function.GetRequestDateTime("DateEnd");
            string Name = Function.GetRequestString("Name");
            string SN = Function.GetRequestString("SN");
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
            if (!string.IsNullOrEmpty(RS))
            {
                list = list.Where(a => a.IP.Contains(RS));
                ViewBag.TxtRS = RS;
            }
            if (!string.IsNullOrEmpty(SN))
            {
                list = list.Where(a => a.SnNumber.Contains(SN));
                ViewBag.TxtSN = SN;
            }

            list = list.OrderBy(p => p.Result);
            return list;
        }

        public ActionResult Export()
        {
            IQueryable<PrizeResultInfo> list = GetListData();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("RS");
            dt.Columns.Add("Name");
            dt.Columns.Add("StoreAdd");
            dt.Columns.Add("Tel");
            dt.Columns.Add("SnNumber");
            dt.Columns.Add("AreaName");
            dt.Columns.Add("CreateDate");
            dt.Columns.Add("Result");
            foreach (var item in list)
            {

                DataRow dr = dt.NewRow();
                dr["ID"] = item.ID;
                dr["RS"] = item.IP;
                dr["Name"] = item.Name;
                dr["StoreAdd"] = item.StoreAdd;
                dr["Tel"] = item.Tel;
                dr["SnNumber"] = item.SnNumber;
                dr["AreaName"] = item.AreaName;
                dr["CreateDate"] = item.CreateDate;
                dr["Result"] = item.Result;
                dt.Rows.Add(dr);
            }
            string FilePathRelative = "/Content/Export/PrizeResult/";
            string FilePathAbs = Server.MapPath(FilePathRelative);
            string FileName = DateTime.Now.ToFileTime().ToString() + ".xlsx";
            if (!Directory.Exists(FilePathAbs))
            {
                Directory.CreateDirectory(FilePathAbs);
            }
            NPOIHelper.Export(dt, FilePathAbs + FileName);
            return Redirect(FilePathRelative + FileName);
        }


    }
}
