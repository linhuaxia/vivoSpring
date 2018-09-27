using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    /// <summary>
    /// 接收消息基类
    /// </summary>
   public abstract class WechatMsgBase
    {
        public WechatMsgBase(string Msg)
        {
            XDocument xmlDoc = XDocument.Parse(Msg);
            this.ToUserName = xmlDoc.Root.Element("ToUserName").Value;
            this.FromUserName= xmlDoc.Root.Element("FromUserName").Value;
            this.CreateTime =Convert.ToInt32(xmlDoc.Root.Element("CreateTime").Value);
            this.MsgType = xmlDoc.Root.Element("MsgType").Value;
            this.MsgId = -1;
            if (xmlDoc.Root.Element("MsgId")!=null)
            {
                this.MsgId = Convert.ToInt64(xmlDoc.Root.Element("MsgId").Value);
            }
            this.XmlDom = Msg;
            SetCurrentInfo(xmlDoc);
        }

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MsgType { get; set; }
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MsgId { get; set; }

        internal string XmlDom { get; set; }

        public abstract string Handle();

        public abstract void SetCurrentInfo(XDocument xmlDoc);

        #region 回复函数

        /// <summary>
        /// 回复文本
        /// </summary>
        /// <param name="requestXML"></param>
        /// <param name="_reMsg"></param>
        /// <returns></returns>
        internal string ResponseText( string _reMsg)
        {
            string ResTxt = "<xml>"
                + "<ToUserName><![CDATA[" + FromUserName + "]]></ToUserName>"
                + "<FromUserName><![CDATA[" + ToUserName + "]]></FromUserName>"
                + "<CreateTime>" +Eval.BLL.WechatService.ConvertDateTimeInt(DateTime.Now) + "</CreateTime>"
                + "<MsgType><![CDATA[text]]></MsgType>"
                + "<Content><![CDATA[" + _reMsg + "]]></Content>"
                + "</xml>";
            return ResTxt;
        }

        ///// <summary>
        ///// 回复图文
        ///// </summary>
        ///// <param name="requestXML"></param>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //private string ResponseTuWen(WechatRequestXML requestXML, List<NewsInfo> list)
        //{
        //    if (null == list || list.Count <= 0)
        //    {
        //        return string.Empty;
        //    }
        //    string ResTxt = "<xml>";
        //    ResTxt += "<ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName>";
        //    ResTxt += "<FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName>";
        //    ResTxt += "<CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime>";
        //    ResTxt += "<MsgType><![CDATA[news]]></MsgType>";
        //    ResTxt += "<ArticleCount>" + list.Count + "</ArticleCount>";
        //    ResTxt += "<Articles>";
        //    string Domain = ConfigHelper.GetAppendSettingValue(ConfigHelper.DoMain);
        //    int Flag = 0;
        //    foreach (var item in list)
        //    {
        //        Flag++;
        //        if (Flag > 8)
        //        {
        //            break;
        //        }
        //        ResTxt += "<item>";
        //        ResTxt += "     <Title><![CDATA[" + item.Title + "]]></Title> ";
        //        ResTxt += "     <Description><![CDATA[" + item.Description + "]]></Description>";
        //        ResTxt += "     <PicUrl><![CDATA[" + Domain + "/file/images/News/" + item.ID + ".png?ramdonID="+Function.GetRand() + "]]></PicUrl>";
        //        ResTxt += "     <Url><![CDATA[" + Domain + "/Wechat/News/Detail.aspx?ID=" + item.ID + "]]></Url>";
        //        ResTxt += "</item>";

        //    }

        //    ResTxt += "</Articles>";
        //    ResTxt += "</xml> ";
        //    return ResTxt;
        //}

        #endregion

    }
}
