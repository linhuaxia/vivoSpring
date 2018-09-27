using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.Model
{

    public partial class ProfilesInfo
    {
        public static class EmailItem
        {

            public const string 回复地址 = "Email回复地址";
            public const string 回复时显示名 = "Email回复时显示名";
            public const string 发件邮箱端口 = "Email发件邮箱端口";
            public const string 发件邮箱地址 = "Email发件邮箱地址";
            public const string 发件邮箱密码 = "Email发件邮箱密码";
            public const string 发件邮件主机 = "Email发件邮件主机";
            public const string 发件人显示名 = "Email发件人显示名";

        }
        public static class Wechat
        {

            public const string MenuJson = "Wechat_MenuJson";
            public const string SubcribeContent = "SubcribeContent";
        }
        public static class OutSideUserSetting
        {

            public const string DepartmentID = "OutSideUserSetting_DepartmentID";
            public const string RuleID = "OutSideUserSetting_RuleID";
        }
        public static class InportSetting
        {

            public const string DefaultRuleIDWhenInport = "_InportSetting_DefaultRuleIDWhenInport";
        }
    }
}

