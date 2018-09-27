using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WeiXin.APIClient.WechatMsgHander
{
    public class WechatMsgVideoShort : WechatMsgVideoBig
    {
        public WechatMsgVideoShort(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            base.SetCurrentInfo(xmlDoc);
        }
    }
}
