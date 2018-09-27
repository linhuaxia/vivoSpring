using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vivo.web.Areas.Wechat.Models
{
    public class WechatHeaderInfo
    {
        public WechatHeaderInfo()
        {
            this.HeadText = "";
            this.LeftURL = "javascript:history.go(-1)"; ;
            this.LeftIcon = "/Content/Wechat/Images/Back.png";
            this.RightText = "/Wechat/Home";
            this.RightIcon = "/Content/Wechat/Images/Home.png";
        }

        public string HeadText { get; set; }
        public string LeftURL { get; set; }
        public string LeftIcon { get; set; }
        public string RightText { get; set; }
        public string RightIcon { get; set; }
    }
}