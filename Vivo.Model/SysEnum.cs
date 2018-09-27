using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vivo.Model
{
    public static class SysEnum
    {
        public static Dictionary<int, string> ToDictionary(Type enumType)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(enumType))
            {
                dic.Add((int)item, Enum.GetName(enumType, (int)item));
            }
            return dic;
        }

        public static string GetName(Type enumType, object value)
        {
            try
            {
                return Enum.GetName(enumType, value);
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public static bool IsParse<T>(string Value)
        {
            try
            {
                var result= (T)Enum.Parse(typeof(T), Value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static T Parse<T>(string Value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), Value);
            }
            catch (Exception)
            {
                return default(T);
            }
        }




        /// <summary>
        /// 用户登录状态
        /// </summary>
        public enum LoginState { 登录成功 = 1, 无权登录, 用户不存在, 密码不正确 }
        public enum ActionHttpMethod { Get = 1, Post }
        public enum PrizeType { 一等奖 = 1, 二等奖 = 2, 爱奇艺会员 = 3, 没中奖 = 4 }
        public class EnableStatus
        {
            public enum EnumEnableStatus { 启用 = 1, 禁用 }
            public static string GetName(bool value)
            {
                if (value)
                {
                    return EnumEnableStatus.启用.ToString();
                }
                return EnumEnableStatus.禁用.ToString();
            }

        }






    }
}