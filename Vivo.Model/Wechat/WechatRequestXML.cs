using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vivo.Model
{
    public class WechatRequestXML
    {
        /// <summary>
        /// 消息接收方微信号，一般为公众平台账号微信号
        /// </summary>
        public string ToUserName { get; set; }


        /// <summary>
        /// 消息发送方微信号
        /// </summary>
        public string FromUserName { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 信息类型 地理位置:location,文本消息:text,消息类型:image
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 信息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Location_X { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Location_Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public string Scale { get; set; }


        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }


        /// <summary>
        /// 图片链接，开发者可以用HTTP GET获取
        /// </summary>
        public string PicUrl { get; set; }

        public string Event { get; set; }
        public string EventKey { get; set; }


        /// <summary>
        /// QRd码扫描结果
        /// </summary>
        public string ScanResult { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ToUserName:").Append(ToUserName).Append("||||<br/>\r\n");
            sb.Append("FromUserName:").Append(FromUserName).Append("||||<br/>\r\n");
            sb.Append("CreateTime:").Append(CreateTime).Append("||||<br/>\r\n");
            sb.Append("MsgType:").Append(MsgType).Append("||||<br/>\r\n");
            sb.Append("Content:").Append(Content).Append("||||<br/>\r\n");
            sb.Append("Location_X:").Append(Location_X).Append("||||<br/>\r\n");
            sb.Append("Location_Y:").Append(Location_Y).Append("||||<br/>\r\n");
            sb.Append("Scale:").Append(Scale).Append("||||<br/>\r\n");
            sb.Append("Label:").Append(Label).Append("||||<br/>\r\n");
            sb.Append("PicUrl:").Append(PicUrl).Append("||||<br/>\r\n");
            sb.Append("Event:").Append(Event).Append("||||<br/>\r\n");
            sb.Append("EventKey:").Append(EventKey).Append("||||<br/>\r\n");
            return sb.ToString();
        }
    }
}
