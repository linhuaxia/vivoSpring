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
    public class WechatMsgEventSubscribe : WechatMsgBase
    {
        public string Event { get; set; }
        public WechatMsgEventSubscribe(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
            ILogInfoService LogBLL = AbstractFactory.CreateLogInfoService();
            IProfilesInfoService ProfilesBLL = AbstractFactory.CreateProfilesInfoService();

            if (Event== "subscribe")
            {
                string SubcribeContent = string.Empty;// ProfilesBLL.GetValue(ProfilesInfo.Wechat.SubcribeContent);
                if (string.IsNullOrEmpty(SubcribeContent))
                {
                    SubcribeContent = "欢迎关注！";
                }
                return ResponseText(SubcribeContent);
            }
            else
            {
                return string.Empty;
            }
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.Event = xmlDoc.Root.Element("Event").Value;
        }
    }
}
