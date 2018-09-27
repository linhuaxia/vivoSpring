using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Tool
{
    public class Md5Helper
    {
        /// <summary>
        /// 标准md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
        }
        public static string GetMD5Result(string input)
        {
            //将要加密的字符串转化为byte类型
            byte[] data = System.Text.Encoding.Unicode.GetBytes(input.ToCharArray());
            //创建一个Md5对象
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //加密Byte[]数组
            byte[] result = md5.ComputeHash(data);
            //将加密后的数组转化为字段
            StringBuilder sb = new StringBuilder();

            foreach (byte b in result)
                sb.Append(b.ToString("X").PadLeft(2, '0'));

            return sb.ToString();
        }


        #region MD5
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="strSource">需要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string Md5Encrypt(string strSource)
        {
            if (string.IsNullOrEmpty(strSource))
            {
                return strSource;
            }
            //把字符串放到byte数组中
            byte[] bytIn = System.Text.Encoding.Default.GetBytes(strSource);
            //建立加密对象的密钥和偏移量        
            byte[] iv = { 102, 16, 93, 156, 78, 4, 218, 32 };//定义偏移量
            byte[] key = { 55, 103, 246, 79, 36, 99, 167, 3 };//定义密钥
            //实例DES加密类
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = iv;
            mobjCryptoService.IV = key;
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            //实例MemoryStream流加密密文件
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            return System.Convert.ToBase64String(ms.ToArray());
        }
        /// <summary>
        /// MD5解密
        /// </summary>
        /// <param name="Source">需要解密的字符串</param>
        /// <returns>MD5解密后的字符串</returns>
        public static string Md5Decrypt(string Source)
        {

            if (string.IsNullOrEmpty(Source))
            {
                return Source;
            }
            //将解密字符串转换成字节数组
            byte[] bytIn = System.Convert.FromBase64String(Source);
            //给出解密的密钥和偏移量，密钥和偏移量必须与加密时的密钥和偏移量相同
            byte[] iv = { 102, 16, 93, 156, 78, 4, 218, 32 };//定义偏移量
            byte[] key = { 55, 103, 246, 79, 36, 99, 167, 3 };//定义密钥
            DESCryptoServiceProvider mobjCryptoService = new DESCryptoServiceProvider();
            mobjCryptoService.Key = iv;
            mobjCryptoService.IV = key;
            //实例流进行解密
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader strd = new StreamReader(cs, Encoding.Default);
            return strd.ReadToEnd();
        }
        #endregion
    }

}