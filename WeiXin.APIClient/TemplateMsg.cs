using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tool;
using Vivo.IBLL;
using Vivo.Model;
using Vivo.BLLFactory;


namespace WeiXin.APIClient
{
    public partial class WechatService
    {
        public class TemplateMsg
        {
            #region 系统的业务逻辑，实在不应该放在这里



            //public bool ResponseTemplateMsg(ResearchPlanUserInfo infoPlanUser, string TargetOpenID)
            //{
            //    WechatTemplateMsgInfo infoTemplateMsg = new WechatTemplateMsgInfo();
            //    infoTemplateMsg.TouserOpenID = string.Empty;
            //    infoTemplateMsg.Template_id = WechatTemplateMsgInfo.GetTemplateMsgID课程提醒(WeiXinConst.AppId);
            //    infoTemplateMsg.URL = ConfigHelper.GetAppendSettingValue(ConfigHelper.WEBDoMain) + "/Wechat/ResearchPlanUser/Confirm?PlanID=" +infoPlanUser.ResearchPlanID;
            //    WechatTemplateMsgItemInfo info = new WechatTemplateMsgItemInfo();
            //    info.DataKey = "first";
            //    info.DataValue = "组织调研参与确认函";
            //    infoTemplateMsg.Data.Add(info);

            //    info = new WechatTemplateMsgItemInfo();
            //    info.DataKey = "keyword1"; //预约项目
            //    info.DataValue = infoPlanUser.ResearchPlanInfo.Name;
            //    infoTemplateMsg.Data.Add(info);

            //    info = new WechatTemplateMsgItemInfo();
            //    info.DataKey = "keyword2"; //预约时间
            //    info.DataValue = infoPlanUser.ResearchPlanInfo.DateBegin.ToString("yyyy-MM-dd");
            //    infoTemplateMsg.Data.Add(info);

            //    info = new WechatTemplateMsgItemInfo();
            //    info.DataKey = "remark"; //remark
            //    info.DataValue = "请及时回复是否参与。";
            //    infoTemplateMsg.Data.Add(info);


            //    infoTemplateMsg.TouserOpenID = TargetOpenID;
            //    return ResponseTemplateMsg(infoTemplateMsg);

            //}

            #endregion

            /// <summary>
            /// 推送模板消息
            /// </summary>
            /// <returns>微信推送完成后的消息ID</returns>
            public bool ResponseTemplateMsg(WechatTemplateMsgInfo infoMsg)
            {
                string URL = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + GetAccessTonken();
                string DataResult = DataHelper.PostHttpData(URL, infoMsg.ToJson());

                LogInfo infoLog = new LogInfo();
                infoLog.Category = "ResponseTemplateMsg";
                infoLog.UserName = DataResult.Length>50?DataResult.Substring(0,49):DataResult;
                infoLog.Detail = infoMsg.ToJson();
                infoLog.AddDate = DateTime.Now;
                infoLog.Serious = 0;
                ILogInfoService LogBLL = AbstractFactory.CreateLogInfoService();
                LogBLL.Create(infoLog);

                return DataResult.Contains("\"errcode\":0,");
            }


        }


    }
}
