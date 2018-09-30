using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;
using Tool;


namespace Vivo.web.Controllers
{
    public class PrizeResultController : BaseController
    {
        public ActionResult C(PrizeResultInfo info)
        {
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
            { info.IP = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]); }
            else
            {
                info.IP = string.Empty;
            }
            if (info.IP.Length>50)
            {
                info.IP = info.IP.Substring(0, 49);
            }

            
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson(1, ""));
            }
            if (string.IsNullOrEmpty(info.Name) || info.Name.Length > 50)
            {
                return Json(new APIJson(2, ""));
            }
            if (string.IsNullOrEmpty(info.StoreAdd))
            {
                return Json(new APIJson(3, ""));
            }
            if (info.StoreAdd.Length>500)
            {
                return Json(new APIJson(4, ""));
            }
            if (!IsMobilePhone(info.Tel))
            {
                return Json(new APIJson(5, "手机号输入不正确，请重新输入!"));
            }
            if (info.SnNumber.Length!=15)
            {
                return Json(new APIJson(6, "SN!"));
            }
            if (string.IsNullOrEmpty(info.AreaName))
            {
                return Json(new APIJson(7, "Area!"));
            }

           var infoExist= PrizeResultBLL.GetList(a => a.SnNumber == info.SnNumber).FirstOrDefault();
            if (null==infoExist)
            {
                return Json(new APIJson(8, "SN not exist!"));
            }
            if (!string.IsNullOrEmpty(infoExist.Name))
            {
                var ExistOBJ = new {infoExist.ID,infoExist.Name,infoExist.IP,infoExist.StoreAdd,infoExist.Tel,infoExist.SnNumber,infoExist.CreateDate,infoExist.Result };
                return Json(new APIJson(-1, "Has Taken!",ExistOBJ));
            }
            infoExist.IP = info.IP;
            infoExist.Name = info.Name;
            infoExist.StoreAdd = info.StoreAdd;
            infoExist.Tel = info.Tel;
            //infoExist.SnNumber=info
            infoExist.AreaName = info.AreaName;
            infoExist.CreateDate = DateTime.Now;
            //infoExist.Result
            if (PrizeResultBLL.Edit(infoExist))
            {
                var ExistOBJ = new { infoExist.ID, infoExist.Name, infoExist.IP, infoExist.StoreAdd, infoExist.Tel, infoExist.SnNumber, infoExist.CreateDate, infoExist.Result };
                return Json(new APIJson(0, "OK!", ExistOBJ));
            }
            else
            {
                return Json(new APIJson(9, "NotSavedYet!"));
            }
        }


        public static bool IsMobilePhone(string input)
        {
            if (input.Length!=10)
            {
                return false;
            }
            try
            {
                Convert.ToInt64(input);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}