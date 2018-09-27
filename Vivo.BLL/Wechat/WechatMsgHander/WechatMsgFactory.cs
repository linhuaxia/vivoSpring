using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Eval.Model;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public static class WechatMsgFactory
    {
        public static WechatMsgBase CreateWechatMsg(string Msg)
        {
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Msg);
                XmlElement rootElement = doc.DocumentElement;
                string MsgType = rootElement.SelectSingleNode("MsgType").InnerText;
                switch (MsgType)
                {
                    case "text": //文本消息
                        return new WechatMsgText(Msg);
                    case "image": //图片
                        return new WechatMsgImage(Msg);
                    case "location": //位置
                        return new WechatMsgLocation(Msg);
                    case "link": //链接
                        return new WechatMsgLink(Msg);
                    case "voice":
                        return new WechatMsgVoice(Msg);
                    case "video":
                        return new WechatMsgVideoBig(Msg);
                    case "shortvideo":
                        return new WechatMsgVideoShort(Msg);
                    case "event": //事件推送 支持V4.5+
                        string Event = rootElement.SelectSingleNode("Event").InnerText;
                        if (Event == "subscribe" || Event == "unsubscribe")
                        {
                            return new WechatMsgEventSubscribe(Msg);
                        }
                        if (Event == "LOCATION")
                        {
                            return new WechatMsgEventLocation(Msg);
                        }
                        if (Event == "CLICK" || Event == "VIEW")
                        {
                            return new WechatMsgEventMenu(Msg);
                        }
                        break;
                    default:
                        return null;

                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfo info = new LogInfo();
                info.Category = "new Base";
                info.UserName = "System";
                info.Detail = ex.Message;
                info.AddDate = DateTime.Now;
                info.Serious = 1;
                info.UserName = "";
                new LogInfoService().Create(info);
                throw;
            }

        }
    }
}
