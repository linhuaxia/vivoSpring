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
    
    public partial class PrizeResultInfo
    {
        public int ID { get; set; }
        public string OpenID { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string SnNumber { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Result { get; set; }
        public string SnApiResult { get; set; }
        public int PlanID { get; set; }
    
        public virtual PlanInfo PlanInfo { get; set; }
    }
}