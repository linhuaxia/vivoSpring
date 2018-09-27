using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eval.Model;

namespace Eval.BLL.Wechat.WechatMsgHander
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
            UserInfoService UserBLL = new UserInfoService();
            WechatMsgInfoService WechatMsgBLL = new WechatMsgInfoService();

            if (WechatMsgBLL.GetList(p => p.MsgId == MsgId).Count() != 0)
            {
                return "success";
            }
            WechatMsgInfo info = new WechatMsgInfo();
            info.CreateUserID = UserBLL.GetList(p => p.Name == DicInfo.Admin).FirstOrDefault().ID;
            info.AddDate = DateTime.Now;
            info.Status = 1;
            info.ToUserName = ToUserName;
            info.FromUserName = FromUserName;
            info.CreateTime = CreateTime;
            info.MsgType = MsgType;
            info.Content = PicUrl;
            info.MsgId = MsgId;
            info.XMLDom = XmlDom;
            WechatMsgBLL.Create(info);

            return "success";
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
            this.PicUrl = xmlDoc.Root.Element("PicUrl").Value;
            this.MediaId = xmlDoc.Root.Element("MediaId").Value;
        }
    }
}
