using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Web;

using System.IO;


namespace Tool
{


    /// <summary>
    /// Provides static methods to read system icons for both folders and files.
    /// </summary>
    /// <example>
    /// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
    /// </example>
    public abstract class DateTimeHelper
    {
        public static DateTime getMonthEnd(int year, int Month)
        {
            try
            {
              return  Convert.ToDateTime(year.ToString() + "-" + Month.ToString() + "-1").AddMonths(1).AddDays(-1.0);
            }
            catch (Exception)
            {
                return Tool.Function.DateZone;   
            }
        }
        /// <summary>
        /// 返回当月第一天的日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static DateTime getMonthFirst(int year, int Month)
        {
            return Convert.ToDateTime(year.ToString() + "-" + Month.ToString() + "-1");
        }

        #region 得到一周的周一和周日的日期
        /// <summary>   
        /// C#日期函数计算本周的周一日期   
        /// </summary>   
        /// <returns></returns>   
        public static DateTime GetMondayDate()
        {
            return GetMondayDate(DateTime.Now);
        }
        /// <summary>   
        /// 计算本周周日的日期   
        /// </summary>   
        /// <returns></returns>   
        public static DateTime GetSundayDate()
        {
            return GetSundayDate(DateTime.Now);
        }
        /// <summary>   
        /// 计算某日起始日期（礼拜一的日期）   
        /// </summary>   
        /// <param name="someDate">该周中任意一天</param>   
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>   
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。   
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary>   
        /// 计算某日结束日期（礼拜日的日期）   
        /// </summary>   
        /// <param name="someDate">该周中任意一天</param>   
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns>   
        public static DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。   
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }
        public static string GetDateOfWeek(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "星期五";
                case DayOfWeek.Monday:
                    return "星期一";
                case DayOfWeek.Saturday:
                    return "星期六";
                case DayOfWeek.Sunday:
                    return "星期日";
                case DayOfWeek.Thursday:
                    return "星期四";
                case DayOfWeek.Tuesday:
                    return "星期二";
                case DayOfWeek.Wednesday:
                    return "星期三";
                default:
                    return "";
            }
        }
        #endregion 
    }

}











