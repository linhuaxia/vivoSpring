using Vivo.BLLFactory;
using Vivo.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Vivo.Model;
using Tool;
using Vivo.web.Areas.MP.Controllers;
using Vivo.web.Areas.Wechat.Models;
using System.Dynamic;

namespace Vivo.web.Areas.Wechat.Controllers
{
    public class TimeLineCommonController : BaseMPController
    {


        [HttpPost]
        public ActionResult Create(TimeLineCommonInfo info)
        {
            TimeLineInfo infoTimeLine = TimeLineBLL.GetList(a => a.ID == info.TimeLineID).FirstOrDefault();
            var infoCommonExist = infoTimeLine.TimeLineCommonInfo.FirstOrDefault(a => a.IsCommon == info.IsCommon && a.CreateUserID == CurrentUser.ID);


            var Result = infoTimeLine.TimeLineCommonInfo.OrderBy(c => c.ID).Select(c => new
            {
                c.ID,
                c.UserInfo.Name,
                c.IsCommon,
                c.Detail
            });


            if (null!=infoCommonExist && !info.IsCommon)
            {
                if (TimeLineCommonBLL.Delete(infoCommonExist))
                {
                    return Json(new APIJson(0, "提交成功", Result));
                }
            }

            info.CreateDate = DateTime.Now;
            info.CreateUserID = CurrentUser.ID;
            if (null==info.Detail)
            {
                info.Detail = string.Empty;
            }
            if (info.IsCommon && string.IsNullOrEmpty(info.Detail))
            {
                return Json(new APIJson(-1, "评论什么？至少写点东西吧"));
            }

            if (TimeLineCommonBLL.Create(info).ID > 0)
            {
                info.UserInfo = CurrentUser;

                
                return Json(new APIJson(0, "提交成功", Result));
            }
            return Json(new APIJson(-1, "提交失败，请重试"));
        }


    }
}
