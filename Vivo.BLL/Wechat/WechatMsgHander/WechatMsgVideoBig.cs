using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgVideoBig : WechatMsgBase
    {
        public string MediaId { get; set; }
        public string ThumbMediaId { get; set; }
        public WechatMsgVideoBig(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.MediaId = xmlDoc.Root.Element("MediaId").Value;
            this.ThumbMediaId = xmlDoc.Root.Element("ThumbMediaId").Value;
        }
    }
}
