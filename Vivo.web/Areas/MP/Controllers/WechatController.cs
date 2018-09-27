using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tool;
using Vivo.BLLFactory;
using Vivo.IBLL;
using Vivo.Model;

namespace Vivo.web.Areas.MP.Controllers
{
    public class WechatController : BaseMPUnFilterController
    {
        public ActionResult Index(int page = 1)
        {

            return View();
        }

        

        public ActionResult UserMenu()
        {
            string  menuJson = ProfilesBLL.GetValue(ProfilesInfo.Wechat.MenuJson,true);
            ViewBag.menuJson = menuJson;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserMenu(string menuJson)
        {
           ProfilesInfo info= ProfilesBLL.Get(ProfilesInfo.Wechat.MenuJson);
            info.Value = menuJson;
            ProfilesBLL.Edit(info);
            
            string URL = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=";
            string token = WeiXin.APIClient.WechatService.GetAccessTonken();

            string msg = "Token={0};<br/>result={1}";
            if (!string.IsNullOrEmpty(token))
            {
                if (token.Length < 10)
                {
                    return Content("token有误");
                }
            }
            var result = DataHelper.PostHttpData(URL + token, menuJson);
            return Content(string.Format(msg,token,result));

        }

        public ActionResult GetTokenFromAPI()
        {
            string token = WeiXin.APIClient.WechatService.GetAccessTonken();
            return Content(token);
        }

    }
}