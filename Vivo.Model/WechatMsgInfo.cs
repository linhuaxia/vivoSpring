//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vivo.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class WechatMsgInfo
    {
        public int ID { get; set; }
        public int CreateUserID { get; set; }
        public System.DateTime AddDate { get; set; }
        public int Status { get; set; }
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public int CreateTime { get; set; }
        public string MsgType { get; set; }
        public string Content { get; set; }
        public long MsgId { get; set; }
        public string XMLDom { get; set; }
    
        public virtual UserInfo UserInfo { get; set; }
    }
}
