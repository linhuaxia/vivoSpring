using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgVoice : WechatMsgBase
    {
        public string MediaId { get; set; }
        public string Format { get; set; }
        public WechatMsgVoice(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.MediaId = xmlDoc.Root.Element("MediaId").Value;
            this.Format = xmlDoc.Root.Element("Format").Value;
        }
    }
}
