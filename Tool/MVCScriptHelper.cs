using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool
{
   public class MVCScriptHelper
    {
        private const string ScriptBegin = "<script type=\"text/javascript\">";
        private const string ScriptEnd = "</script>";
        public static string JoinScriptLab(string JS)
        {
            return ScriptBegin + JS + ScriptEnd;
        }

        /// <summary>
        /// 刷新指定页面
        /// </summary>
        /// <param name="Target">frame target，如果直接刷新当前页面，可输入null或空字符串</param>
        /// <returns></returns>
        public static string Refresh(string Target)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Target))
            {
                sb.Append("top.window.parent.frames['").Append(Target).Append("'].location=top.window.parent.frames['").Append(Target).Append("'].location");
            }
            else
            {
                sb.Append("location.href=location.href;");
            }
            return JoinScriptLab(sb.ToString());
        }

        public static string Alert(string Msg)
        {
            string JS = "alert('" + Msg + "');";
            return JoinScriptLab(JS);
        }
        public static string AlertBack(string Msg)
        {
            string JS = "alert('" + Msg + "');";
            JS += "history.go(-1)";
            return JoinScriptLab(JS);
        }

        public static string AlertRedirect(string Msg, string Target, string URL)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('").Append(Msg).Append("');");
            if (!string.IsNullOrEmpty(Target))
            {
                sb.Append("top.window.parent.frames['").Append(Target).Append("'].");
            }
            sb.Append("location.href='").Append(URL).Append("'");
            return JoinScriptLab(sb.ToString());

        }
        public static string AlertRefresh(string Msg, string Target)
        {
            string js = Alert(Msg);
            js += Refresh(Target);
            return js;

        }

    }
}
