using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Eval.Model;
using Eval.IDAL;
using Eval.DALFactory;


namespace Eval.BLL.Wechat.WechatMsgHander
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
            UserInfoService UserBLL = new UserInfoService();
            WechatMsgInfoService WechatMsgBLL = new WechatMsgInfoService();

            UserInfo infoUser = UserBLL.GetList(p => p.WechatOpenID == FromUserName).FirstOrDefault();
            if (infoUser==null)
            {
                infoUser= UserBLL.GetList(p => p.Name == DicInfo.Admin).FirstOrDefault();
            }
            if (WechatMsgBLL.GetList(p => p.MsgId == MsgId).Count() != 0)
            {
                return "success";
            }
            WechatMsgInfo info = new WechatMsgInfo();
            info.CreateUserID = infoUser.ID;
            info.AddDate = DateTime.Now;
            info.Status = 1;
            info.ToUserName = ToUserName;
            info.FromUserName = FromUserName;
            info.CreateTime = CreateTime;
            info.MsgType = MsgType;
            info.Content = Content;
            info.MsgId =  MsgId;
            info.XMLDom = XmlDom;
            WechatMsgBLL.Create(info);

            var PowerKey = string.Empty;// PowerInfo.P_通知管理.PP微信消息.PPP微信会话管理.接口新消息推送;

            var listUser= UserBLL.GetList(p => p.RuleInfo.Any(r => r.PowerActionInfo.Any(pa=>pa.NewID== PowerKey)));
            foreach (var item in listUser)
            {
                if (string.IsNullOrEmpty(item.WechatOpenID))
                {
                    continue;
                }
                new Eval.BLL.WechatService.TemplateMsg().ResponseTemplateMsgNewWechatMsg(info, item.WechatOpenID);
            }
            return ResponseText("消息收到，相关人员将稍候回复您");
        }

        public override void SetCurrentInfo(XDocument xmlDoc)
        {
               this.Content = xmlDoc.Root.Element("Content").Value;
        }

    }
}
