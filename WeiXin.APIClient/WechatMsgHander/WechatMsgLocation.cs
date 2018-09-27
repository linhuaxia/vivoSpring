using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WeiXin.APIClient.WechatMsgHander
{
    public class WechatMsgLocation : WechatMsgBase
    {
        public decimal Location_X { get; set; }
        public decimal Location_Y { get; set; }
        public int Scale { get; set; }
        public string Label { get; set; }
        public WechatMsgLocation(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            string vResponseText = string.Format("你的经纬度是:lon:{0};lat:{1}，地址：{2}", Location_Y, Location_X,Label);

            return ResponseText(vResponseText);
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.Location_X = Convert.ToDecimal(xmlDoc.Root.Element("Location_X").Value);
            this.Location_Y = Convert.ToDecimal(xmlDoc.Root.Element("Location_Y").Value);
            this.Scale = Convert.ToInt16(xmlDoc.Root.Element("Scale").Value);
            this.Label = xmlDoc.Root.Element("Label").Value;
        }
    }
}
