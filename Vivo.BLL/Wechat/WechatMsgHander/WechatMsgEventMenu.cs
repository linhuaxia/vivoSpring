using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgEventMenu : WechatMsgBase
    {
        public string Event { get; set; }
        public string EventKey { get; set; }
        public WechatMsgEventMenu(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.Event = xmlDoc.Root.Element("Event").Value;
            this.EventKey = xmlDoc.Root.Element("EventKey").Value;
        }
    }
}
