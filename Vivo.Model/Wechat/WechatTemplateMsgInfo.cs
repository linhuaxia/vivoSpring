using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vivo.Model
{

    /// <summary>
    /// 微信模板消息
    /// </summary>
    public class WechatTemplateMsgInfo
    {
        public WechatTemplateMsgInfo()
        {
            this.URL = string.Empty;
            this.Topcolor = "FF0000";
            this.Data = new List<WechatTemplateMsgItemInfo>();
        }

        /// <summary>
        /// 接收消息的用户OPENID
        /// </summary>
        public string TouserOpenID { get; set; }

        /// <summary>
        /// 消息模板ID
        /// </summary>
        public string Template_id { get; set; }

        /// <summary>
        /// 消息所打开的连接
        /// </summary>
        public string URL { get; set; }
        public string Topcolor { get; set; }
        public List<WechatTemplateMsgItemInfo> Data { get; set; }

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"touser\":\"{0}\",", this.TouserOpenID));
            sb.Append(string.Format("\"template_id\":\"{0}\",", this.Template_id));
            sb.Append(string.Format("\"url\":\"{0}\",", this.URL));
            sb.Append(string.Format("\"topcolor\":\"#{0}\",", this.Topcolor));
            sb.Append("\"data\": {");
            foreach (var item in Data)
            {
                sb.Append(string.Format("\"{0}\":", item.DataKey));
                sb.Append("{");
                sb.Append(string.Format("\"value\":\"{0}\",", item.DataValue));
                sb.Append(string.Format("\"color\":\"#{0}\"", item.Color));
                sb.Append("},");
            }
           return sb.ToString().TrimEnd(',') + "}}";
        }

        public const string TestOPENID = "wx225f8a34d1cda4d8";

        public static string GetTemplateMsgID课程提醒(string AppID)
        {
            if (AppID == TestOPENID)//测试号
            {
                return "ODbNqyppTpsiah4ec4ZXli6SlkTs4q7aoOLwX8KJ2LE";
            }
            return "zsMwts_LKFwQ1CmSBwiXAYK9pbFh1uFyc8dPIrP6uhg";
        }

    }
    /// <summary>
    /// 微信模板消息详细项
    /// </summary>
    public class WechatTemplateMsgItemInfo
    {
        public WechatTemplateMsgItemInfo()
        {
            this.Color = "170D92";
        }
        public string DataKey { get; set; }
        public string DataValue { get; set; }
        public string Color { get; set; }


    }

}
