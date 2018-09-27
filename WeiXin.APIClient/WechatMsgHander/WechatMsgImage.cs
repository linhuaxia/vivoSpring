using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vivo.Model;
using Vivo.IBLL;
using Vivo.BLLFactory;

namespace WeiXin.APIClient.WechatMsgHander
{
    public class WechatMsgImage : WechatMsgBase
    {
        public string PicUrl { get; set; }

        public string MediaId { get; set; }
        public WechatMsgImage(string Msg) : base(Msg)
        {

        }

        public override string Handle()
        {
            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
            IWechatMsgInfoService WechatMsgBLL = AbstractFactory.CreateWechatMsgInfoService();

           

            return "success";
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.PicUrl = xmlDoc.Root.Element("PicUrl").Value;
            this.MediaId = xmlDoc.Root.Element("MediaId").Value;
        }
    }
}
