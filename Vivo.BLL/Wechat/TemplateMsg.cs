using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tool;
using Eval.IBLL;
using Eval.Model;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;

namespace Eval.BLL
{
    public partial class WechatService
    {
        public class TemplateMsg
        {
            #region 系统的业务逻辑，实在不应该放在这里


            /// <summary>
            /// 有用户向公众号发送消息时，通知相应的管理员
            /// </summary>
            /// <returns></returns>
            public int ResponseTemplateMsgNewWechatMsg(WechatMsgInfo infoWechatMsg,string TargetOpenID)
            {
                WechatTemplateMsgInfo infoTemplateMsg = new WechatTemplateMsgInfo();
                infoTemplateMsg.TouserOpenID = string.Empty;
                infoTemplateMsg.Template_id = WechatTemplateMsgInfo.GetTemplateMsgID工作提醒(WeiXinConst.AppId);
                infoTemplateMsg.URL = ConfigHelper.GetAppendSettingValue(ConfigHelper.WEBDoMain) + "/Wechat/WechatMsg/Talk?FromUserName=" + infoWechatMsg.FromUserName;
                WechatTemplateMsgItemInfo info = new WechatTemplateMsgItemInfo();
                info.DataKey = "first";
                info.DataValue = "收到新消息";
                infoTemplateMsg.Data.Add(info);

                info = new WechatTemplateMsgItemInfo();
                info.DataKey = "keyword1"; //任务编号
                info.DataValue = infoWechatMsg.ID.ToString();
                infoTemplateMsg.Data.Add(info);

                info = new WechatTemplateMsgItemInfo();
                info.DataKey = "keyword2"; //任务类型
                info.DataValue = "消息跟进处理";
                infoTemplateMsg.Data.Add(info);

                info = new WechatTemplateMsgItemInfo();
                info.DataKey = "keyword3"; //任务描述
                info.DataValue = infoWechatMsg.Content;
                infoTemplateMsg.Data.Add(info);

                info = new WechatTemplateMsgItemInfo();
                info.DataKey = "remark"; //remark
                info.DataValue = "请参阅消息详细，如果回复，请在收到消息48小时内进行。";
                infoTemplateMsg.Data.Add(info);


                infoTemplateMsg.TouserOpenID = TargetOpenID;
                int Result = ResponseTemplateMsg(infoTemplateMsg);
                return Result;

            }


            #endregion

            /// <summary>
            /// 推送模板消息
            /// </summary>
            /// <returns>微信推送完成后的消息ID</returns>
            public int ResponseTemplateMsg(WechatTemplateMsgInfo infoMsg)
            {
                string URL = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + GetAccessTonken();
                string DataResult = DataHelper.PostHttpData(URL, infoMsg.ToJson());
                if (DataResult.Contains("\"errcode\":0,"))
                {
                    DataResult = DataResult.Substring(DataResult.IndexOf("\"msgid\":") + 8);
                    return Function.ConverToInt(DataResult.Replace("}", ""));
                }
                return 0;
            }


        }


    }
}
