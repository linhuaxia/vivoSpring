using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgLink : WechatMsgBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public WechatMsgLink(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.Title = xmlDoc.Root.Element("Title").Value;
            this.Description = xmlDoc.Root.Element("Description").Value;
            this.Url = xmlDoc.Root.Element("Url").Value;
        }
    }
}
