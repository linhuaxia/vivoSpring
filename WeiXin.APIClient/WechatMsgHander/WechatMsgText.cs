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
    /// <summary>
    /// 文本消息
    /// </summary>
    public class WechatMsgText : WechatMsgBase
    {
        public string Content { get; set; }
        public WechatMsgText(string Msg) : base(Msg) 
        {

        }
        public override string Handle()
        {
            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
            IWechatMsgInfoService WechatMsgBLL = AbstractFactory.CreateWechatMsgInfoService();

            return ResponseText("消息收到，相关人员将稍候回复您");
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
               this.Content = xmlDoc.Root.Element("Content").Value;
        }

    }
}
