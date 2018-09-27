using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eval.Model;

namespace Eval.BLL.Wechat.WechatMsgHander
{
    public class WechatMsgEventSubscribe : WechatMsgBase
    {
        public string Event { get; set; }
        public WechatMsgEventSubscribe(string Msg) : base(Msg)
        {
        }

        public override string Handle()
        {
            UserInfoService UserBLL = new UserInfoService();
            LogInfoService LogBLL = new LogInfoService();
            ProfilesInfoService ProfilesBLL = new ProfilesInfoService();

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
