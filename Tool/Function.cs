using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Xml.Xsl;
using System.Text.RegularExpressions;
using System.Data.OleDb;

namespace Tool
{
    /// <summary>
    /// 基本工具类
    /// </summary>
    public abstract class Function
    {
        public static readonly DateTime DateZone = new DateTime(1970, 1, 1);
        public static readonly int ErrorNumber = -999999999;
        public const string Admin = "admin";


        /// <summary>
        /// 格式化IP地址
        /// </summary>
        /// <param name="IP地址">IP地址</param>
        public static string FormatIP(string IP)
        {
            try
            {
                IPAddress NIP = IPAddress.Parse(IP);
                return NIP.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static string PagingWap(int PageSize, int TotalCount, int PageIndex, string url)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"height: 53px; \" title=\"无用的元素，只是点位，当列表数据很长时，fix的分布元素会挡住最后一行的数据嘛\"></div>");
            sb.Append("<div class=\"DivPager\"  id=\"DivPager\">");
            sb.Append("    <table cellpadding='0' cellspacing=\"0\" width=\"100%\" border=\"0\">");
            sb.Append("        <tr>");
            sb.Append("            <td width=\"65\">");
            sb.Append("                 <a  href=\"[pageup]\"><img src=\"/images/btnLeft.png\" style=\"width:20px\" /></a>");
            sb.Append("            </td>");
            sb.Append("            <td style='text-align:center;'>");
            sb.Append("                <span id=\"SpanPageCurrent\">[SpanPageCurrent]</span> /<span id=\"SpanPageTotal\">[SpanPageTotal]</span>");
            sb.Append("            </td>");
            sb.Append("            <td width=\"65\">");
            sb.Append("                 <a  href=\"[pagedown]\"><img src=\"/images/btnRight.png\" style=\"width:20px\" /></a>");
            sb.Append("            </td>");
            sb.Append("        </tr>");
            sb.Append("    </table>");
            sb.Append("</div>");

            PageIndex = PageIndex <= 0 ? 1 : PageIndex;//传过来页码本身修正一下
            int MaxPage = 0; //总页数
            if (TotalCount % PageSize == 0)
                MaxPage = TotalCount / PageSize;
            else
                MaxPage = TotalCount / PageSize + 1;
            sb= sb.Replace("[SpanPageCurrent]", PageIndex.ToString());
            sb = sb.Replace("[SpanPageTotal]",MaxPage.ToString());
            if (PageIndex<=1)
            {
                sb = sb.Replace("[pageup]","javascript:void(0);");
            }
            else
            {
                sb = sb.Replace("[pageup]", "?page="+(PageIndex-1)+url);
            }

            if (PageIndex>=MaxPage)
            {
                sb = sb.Replace("[pagedown]", "javascript:void(0);");
            }
            else
            {
                sb = sb.Replace("[pagedown]", "?page=" + (PageIndex +1 ) + url);
            }

            return sb.ToString();


        }


        /// <summary>
        /// 获取传递参数(字符串)
        /// </summary>
        public static string GetRequestString(string input)
        {
            string temp = HttpContext.Current.Request.Form[input];
            if (string.IsNullOrEmpty(temp)) temp = HttpContext.Current.Request.QueryString[input];
            if (string.IsNullOrEmpty(temp)) temp = string.Empty;

            //temp = ReplaceText(temp, @"\s+", " ").Trim();

            return System.Web.HttpUtility.HtmlDecode(temp);
        }

        public static DateTime GetRequestDateTime(string input)
        {
            DateTime Retu_Date = Function.DateZone;
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    Retu_Date = Convert.ToDateTime(GetRequestString(input));
                }
                catch (Exception)
                {
                    Retu_Date = Function.DateZone;
                }
            }
            return Retu_Date;
        }
        public static DateTime? GetRequestDateTimeNullable(string input)
        {
            DateTime Retu_Date = GetRequestDateTime(input);
            if (DateZone==Retu_Date)
            {
                return null;
            }
            return Retu_Date;
        }



        /// <summary>
        /// 获取传递参数(数字)
        /// </summary>
        public static int GetRequestInt(string input)
        {
            return GetRequestInt(input, ErrorNumber);
        }

        public static int GetRequestInt(string input, int DefaultVaule)
        {
            return ConverToInt(GetRequestString(input), DefaultVaule);
        }
        public static int? GetRequestIntNullable(string input)
        {
            int value = GetRequestInt(input);
            if (value == Function.ErrorNumber)
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 整型转换
        /// </summary>
        public static Int32 ConverToInt<T>(T input)
        {
            Int32 temp = 0;

            try
            {
                temp = Convert.ToInt32(input);
            }
            catch (Exception e)
            {
                temp = Function.ErrorNumber;
            }

            return temp;
        }

        /// <summary>
        /// 整型转换
        /// </summary>
        public static Int32 ConverToInt<T>(T input,int DefalutValue)
        {
            Int32 temp = DefalutValue;

            try
            {
                temp = Convert.ToInt32(input);
            }
            catch (Exception e)
            {
                temp = DefalutValue;
            }

            return temp;
        }

        /// <summary>
        /// 小数转换
        /// </summary>
        public static float ConverToSingle<T>(T input)
        {
            float temp = 0;

            try
            {
                temp = Convert.ToSingle(input);
            }
            catch (Exception e)
            {
                temp = Function.ErrorNumber;
            }

            return temp;
        }

        /// <summary>
        /// decimal转换
        /// </summary>
        public static decimal ConverToDecimal<T>(T input)
        {
            decimal temp = 0;

            try
            {
                temp = Convert.ToDecimal(Convert.ToDecimal(input).ToString("G0"));
            }
            catch (Exception e)
            {
                temp = Function.ErrorNumber;
            }

            return temp;
        }

        public static decimal ConverToDecimal<T>(T input, decimal DefalutValue)
        {
            decimal temp = DefalutValue;

            try
            {
                temp = Convert.ToDecimal(Convert.ToDecimal(input).ToString("G0"));
            }
            catch (Exception e)
            {
                temp = DefalutValue;
            }

            return temp;
        }



        /// <summary>
        /// 字符转换
        /// </summary>
        public static string ConverToString<T>(T input)
        {
            string temp = string.Empty;

            try
            {
                temp = Convert.ToString(input);
            }
            catch (Exception e)
            {
                temp = string.Empty;
            }

            return temp;
        }

        /// <summary>
        /// 日期转换
        /// </summary>
        public static DateTime ConverToDateTime<T>(T input)
        {
            DateTime temp = new DateTime();
            try
            {
                temp = Convert.ToDateTime(input);
            }
            catch (Exception e)
            {
                temp = Function.DateZone;
            }

            return temp;
        }
        /// <summary>
        /// 日期转换
        /// </summary>
        public static DateTime ConverToDateTime<T>(T input,DateTime DefalutValue)
        {
            DateTime temp = DefalutValue;
            try
            {
                temp = Convert.ToDateTime(input);
            }
            catch (Exception e)
            {
                temp = DefalutValue;
            }

            return temp;
        }


        /// <summary>
        /// 替换URL参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ReplaceURLParms(string url, string key, string val)
        {
            var exp = key + "=([^&]*)";
            var r = new System.Text.RegularExpressions.Regex(exp);
            if (r.IsMatch(url))
            {
                url = r.Replace(url, key + "=" + val);
            }
            else
            {
                url += url.Contains("?") ? "&" : "?";
                url += key + "=" + val;
            }
            //url = Regex.Replace(url, exp, para + "=" + val);

            return url;
        }




        /// <summary>
        /// 排除符号
        /// </summary>
        public static string ClearText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = ReplaceText(input, @"[^a-zA-Z0-9 \u4e00-\u9fa5\.]", string.Empty);
                return string.IsNullOrEmpty(input) ? string.Empty : input.Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 正则替换
        /// </summary>
        /// <param name="input">要替换的字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="replacement">替换成的字符</param>
        /// <returns></returns>
        public static string ReplaceText(string input, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string temp = rgx.Replace(input, replacement);
            return temp;
        }

        /// <summary>
        /// 多出几个字条就用。。代替距
        /// </summary>
        /// <param name="old">要截的字符串</param>
        /// <param name="cutmuch">截几个</param>
        /// <returns></returns>
        public static string Cuter<T>(T input, int cutmuch)
        {
            string old = input.ToString();
            if (cutmuch > old.Length || string.IsNullOrEmpty(old))
            {
                return old;
            }
            return old.Substring(0, cutmuch) + "...";

        }

        /// <summary>
        /// 去掉html
        /// </summary>
        /// <param name="HtmlCode"></param>
        /// <returns></returns>
        public static string RemoveHTML<T>(T input)
        {
            string HtmlCode = input.ToString();
            Regex re = new Regex(@"<img([\s\S]*?)>", RegexOptions.IgnoreCase);
            string MatchVale = HtmlCode;
            foreach (Match s in Regex.Matches(HtmlCode, "<.+?>"))
            {
                if (re.IsMatch(s.Value))
                {
                    MatchVale = MatchVale.Replace(s.Value, "[图片]");
                }
                MatchVale = MatchVale.Replace(s.Value, "");
            }
            return MatchVale;
        }

        /// <summary>
        /// 验证内容区域中是否符合表达式
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="strRegex">正则表达式</param>
        /// <returns>符合返回真，不符合返回假</returns>
        public static bool IsHasMatch(string content, string strRegex)
        {
            if (!Regex.IsMatch(content, strRegex))
            {
                return false;
            }
            return true;
        }

        private static Random rand = new Random();
        public static string GetRand()
        {
            return DateTime.Now.ToString("ddHHmmss") + rand.Next(100, 999).ToString();
        }

        public static decimal GetDistance(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
        {
            double lat1 = (Math.PI / 180) * latitudeA;
            double lat2 = (Math.PI / 180) * latitudeB;

            double lon1 = (Math.PI / 180) * longitudeA;
            double lon2 = (Math.PI / 180) * longitudeB;

            //      double Lat1r = (Math.PI/180)*(gp1.getLatitudeE6()/1E6);  
            //      double Lat2r = (Math.PI/180)*(gp2.getLatitudeE6()/1E6);  
            //      double Lon1r = (Math.PI/180)*(gp1.getLongitudeE6()/1E6);  
            //      double Lon2r = (Math.PI/180)*(gp2.getLongitudeE6()/1E6);  

            //地球半径  
            double R = 6371;

            //两点间距离 km，如果想要米的话，结果*1000就可以了  
            double d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)) * R;

            return (decimal)(d * 1000);
        }

        public abstract class Request
        {
            /// <summary>
            /// 获取传递参数(字符串)
            /// </summary>
            public static string GetRequestString(HttpContext context, string input)
            {
                string temp = context.Request.Form[input];
                if (string.IsNullOrEmpty(temp)) temp = context.Request.QueryString[input];
                if (string.IsNullOrEmpty(temp)) temp = string.Empty;

                //temp = ReplaceText(temp, @"\s+", " ").Trim();

                return System.Web.HttpUtility.HtmlDecode(temp);
            }



            public static DateTime GetRequestDateTime(HttpContext context, string input)
            {
                DateTime Retu_Date = Function.DateZone;
                if (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        Retu_Date = Convert.ToDateTime(GetRequestString(context, input));
                    }
                    catch (Exception)
                    {
                        Retu_Date = Function.DateZone;
                    }
                }
                return Retu_Date;
            }




            /// <summary>
            /// 获取传递参数(数字)
            /// </summary>
            public static int GetRequestInt(HttpContext context, string input)
            {
                return ConverToInt(GetRequestString(context, input));
            }

        }

    }
}
