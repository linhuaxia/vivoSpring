using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tool;
using Vivo.IBLL;
using Vivo.Model;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;


namespace WeiXin.APIClient
{
    public static partial class WechatService
    {
        public static class WechatFile
        {
            /// <SUMMARY> 
            /// 下载保存多媒体文件,返回多媒体保存路径 
            /// </SUMMARY> 
            /// <PARAM name="ACCESS_TOKEN"></PARAM> 
            /// <PARAM name="MEDIA_ID"></PARAM> 
            ///  <param name="SaveMapPath">保存的绝对目录，最后需要加上/</param>
            /// <param name="SaveName"></param>
            /// <returns></returns>
            public static string GetMultimedia(string ACCESS_TOKEN, string MEDIA_ID, string SaveMapPath, string SaveName)
            {
                string file = string.Empty;
                string content = string.Empty;
                string strpath = string.Empty;
                string savepath = string.Empty;
                string stUrl = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + ACCESS_TOKEN + "&media_id=" + MEDIA_ID;

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(stUrl);

                req.Method = "GET";
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();

                    strpath = myResponse.ResponseUri.ToString();
                    // WriteLog("接收类别://" + myResponse.ContentType);
                    WebClient mywebclient = new WebClient();
                    savepath = SaveMapPath + SaveName;
                    //WriteLog("路径://" + savepath);
                    if (File.Exists(savepath))
                    {
                        File.Delete(savepath);
                    }

                    try
                    {
                        mywebclient.DownloadFile(strpath, savepath);
                        file = savepath;
                    }
                    catch (Exception ex)
                    {
                        file = "";
                    }

                }
                return file;
            }
        }

    }
}
