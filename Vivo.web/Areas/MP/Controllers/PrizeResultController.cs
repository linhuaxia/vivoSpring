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
            IPagedList<PrizeResultInfo> result = list.ToPagedList(page, 9999);
            return View(result);
        }

        private IQueryable<PrizeResultInfo> GetListData()
        {
            var list = PrizeResultBLL.GetList(p => true);

            list = list.OrderBy(p => p.Result);
            return list;
        }

        //public ActionResult Export()
        //{
        //    IQueryable<PrizeResultInfo> list = GetListData();
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("ID");
        //    dt.Columns.Add("OpenID");
        //    dt.Columns.Add("姓名");
        //    dt.Columns.Add("电话");
        //    dt.Columns.Add("SN号");
        //    dt.Columns.Add("抽奖时间");
        //    dt.Columns.Add("抽奖结果");
        //    dt.Columns.Add("限定活动");
        //    foreach (var item in list)
        //    {

        //        DataRow dr = dt.NewRow();
        //        dr["ID"] = item.ID;
        //        dr["OpenID"] = item.OpenID;
        //        dr["姓名"] = item.Name;
        //        dr["电话"] = item.Tel;
        //        dr["SN号"] = item.SnNumber;
        //        dr["抽奖时间"] = item.CreateDate;
        //        dr["抽奖结果"] = item.Result;
        //        dr["限定活动"] = string.Format("{0}|{1}|{2}到{3}",
        //             item.PlanInfo.AgenterName,
        //             item.PlanInfo.MarketLevel,
        //             item.PlanInfo.DateBegin.ToString("MM-dd HH"),
        //             item.PlanInfo.DateEnd.ToString("MM-dd HH"));
        //        dt.Rows.Add(dr);
        //    }
        //    string FilePathRelative = "/Content/Export/PrizeResult/";
        //    string FilePathAbs = Server.MapPath(FilePathRelative);
        //    string FileName = DateTime.Now.ToFileTime().ToString()+".xlsx";
        //    if (!Directory.Exists(FilePathAbs))
        //    {
        //        Directory.CreateDirectory(FilePathAbs);
        //    }
        //    NPOIHelper.Export(dt, FilePathAbs + FileName);
        //    return Redirect(FilePathRelative + FileName);
        //}


    }
}
