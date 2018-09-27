using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tool;
using Vivo.IBLL;
using Vivo.BLLFactory;
using Vivo.Model;

namespace Vivo.web.Controllers
{
    public class WeChatInterfaceController : Controller
    {
        public ActionResult Index()
        {
            string echostr = Function.GetRequestString("echostr");//微信后台绑定验证
            if (!string.IsNullOrEmpty(echostr))
            {
                return Content(echostr);
            }

            var weiXinInfo = new WeiXin.APIClient.WechatService.weixin();

            if (weiXinInfo.CheckSignature() && !string.IsNullOrEmpty(echostr))
            {
                return Content(echostr);
            }
            if (Request.HttpMethod.ToLower() != "post")
            {
                return Content("只接受post请求");
            }

            System.IO.Stream s = Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            var postStr = System.Text.Encoding.UTF8.GetString(b);
            if (!string.IsNullOrEmpty(postStr))
            {
                LogInfo info = new LogInfo();
                info.Category = "ResponseWechatMsg";
                info.UserName = "System";
                info.AddDate = DateTime.Now;
                info.Serious = 1;
                info.UserName = "";
                try
                {
                    string Result = weiXinInfo.Handle(postStr);
                    return Content(Result);
                }
                catch (Exception ex)
                {
                    info.Detail += "||||ex.Message" + ex.Message;
                    //new LogInfoService().Create(info);
                    ILogInfoService LogBLL = AbstractFactory.CreateLogInfoService();
                    LogBLL.Create(info);
                }
            }


            return Content("空白请求");
        }

    }
}