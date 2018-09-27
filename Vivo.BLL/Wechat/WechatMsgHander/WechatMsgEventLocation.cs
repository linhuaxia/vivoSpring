using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgEventLocation : WechatMsgBase
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Precision { get; set; }
        public WechatMsgEventLocation(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            return string.Empty;
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.Latitude = Convert.ToDecimal(xmlDoc.Root.Element("Latitude").Value);
            this.Longitude = Convert.ToDecimal(xmlDoc.Root.Element("Longitude").Value);
            this.Precision = Convert.ToDecimal(xmlDoc.Root.Element("Precision").Value);
        }
    }
}
