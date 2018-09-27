using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vivo.Model;
using Vivo.IDAL;
using Vivo.IBLL;
using Tool;

namespace Vivo.BLL
{
    public partial class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        /// <summary>
        /// 当前登录员工（sharp_员工ID 形式后加密）
        /// </summary>
        /// <returns></returns>
        public  UserInfo GetCurrent()
        {

            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["UserInfo"] != null)
            {
                return (UserInfo)System.Web.HttpContext.Current.Session["UserInfo"];
            }
            int ID = GetCurrentEmployeeID();
            if (ID > 0)
            {
                
                UserInfo info = CurrentDAL.GetList(p => p.ID == ID).FirstOrDefault();
                if (null == info)
                {
                    return null;
                }
                if (null != System.Web.HttpContext.Current.Session)
                {
                    System.Web.HttpContext.Current.Session["UserInfo"] = info;
                }

                return info;
            }
            return null;
        }
        /// <summary>
        /// 当前登录员工ID
        /// </summary>
        /// <returns></returns>
        private  int GetCurrentEmployeeID()
        {
            string Md5ID = CookiesHelper.GetCookieValue("Employee");
            if (string.IsNullOrEmpty(Md5ID))
            {
                return 0;
            }
            Md5ID = Md5Helper.Md5Decrypt(Md5ID);
            int indexof = Md5ID.IndexOf("sharp_");
            if (indexof == 0)
            {
                Md5ID = Md5ID.Replace("sharp_", "");
            }
            int ID = Tool.Function.ConverToInt(Md5ID);
            if (ID < 0)
            {
                return 0;
            }
            return ID;
        }
        /// <summary>
        /// 当前登录员工ID
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentEmployeeName()
        {
            string Md5ID = CookiesHelper.GetCookieValue("EmployeeName");
            if (string.IsNullOrEmpty(Md5ID))
            {
                return Md5ID;
            }
            Md5ID = Md5Helper.Md5Decrypt(Md5ID);
            int indexof = Md5ID.IndexOf("sharp_");
            if (indexof == 0)
            {
                Md5ID = Md5ID.Replace("sharp_", "");
            }
            return Md5ID;
        }


        public void SetUserInfo(int RemberHour, UserInfo info)
        {
            CookiesHelper.AddCookie("Employee", Md5Helper.Md5Encrypt("sharp_" + info.ID.ToString()), DateTime.Now.AddHours(RemberHour));
            CookiesHelper.AddCookie("EmployeeName", Md5Helper.Md5Encrypt("sharp_" + info.WechatOpenID), DateTime.Now.AddHours(RemberHour));
            System.Web.HttpContext.Current.Session["UserInfo"] = info;
        }

        public void Logout()
        {
            CookiesHelper.SetCookie("Employee", "", DateTime.Now.AddDays(-111));
            CookiesHelper.SetCookie("EmployeeName", "", DateTime.Now.AddDays(-111));
            System.Web.HttpContext.Current.Session.RemoveAll();

        }


    }
}
 