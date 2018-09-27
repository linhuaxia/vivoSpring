using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Web.UI;
using System.Data.SqlClient;


namespace Tool
{


    [Serializable]
    public class DocHelper
    {







        /// <summary>
        /// 检查文件,如果文件不存在则创建 
        /// </summary>
        /// <param name="FilePath"></param>
        private static void ExistsFile(string FilePath)
        {
            FilePath = FilePath.Replace("/", "\\");
            //以上写法会报错,详细解释请看下文.........
            if (!File.Exists(FilePath))
            {
                string Dir = FilePath.Substring(0, FilePath.LastIndexOf("\\") + 1);
                if (!Directory.Exists(Dir))
                {
                    Directory.CreateDirectory(Dir);
                }
                // File.Create(FilePath);
                FileStream fs = File.Create(FilePath);
                fs.Close();
            }
        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="FileFullName">完事文件路径及文件名</param>
        /// <param name="utf"></param>
        /// <returns></returns>
        public static string Read(string FileFullName, System.Text.Encoding Encoding)
        {
            string input = "";
            ExistsFile(FileFullName);//检查文件是否存在 
                                 //读取文件 
            try
            {
                using (StreamReader sr = new StreamReader(FileFullName, Encoding))
                {
                    input = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return input;
        }
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="FilePath">相对系统根目录的路径</param>
        public static string Read(string FilePath)
        {
            return Read(FilePath, System.Text.Encoding.Default);
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="FilePath">相对站点根目录的路径</param>
        /// <param name="NewString"></param>
        /// <returns></returns>
        public static bool Write(string FilePath, string NewString)
        {
            return Write(FilePath, NewString, false);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="FilePath">相对站点根目录的路径</param>
        public static bool Write(string FilePath, string NewString, bool utf)
        {
            ExistsFile(FilePath);//检查文件是否存在 
            StreamWriter sr = new StreamWriter(FilePath, false, utf ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default);
            try
            {
                sr.Write(NewString);
                sr.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }





    }
}