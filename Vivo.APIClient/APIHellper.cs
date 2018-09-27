using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.APIClient
{
    public class APIHellper
    {
        public static class ConstConfig
        {
            /// <summary>
            /// api服务器地址
            /// </summary>
            public static readonly string APIURL = Tool.ConfigHelper.GetAppendSettingValue("ApiUrl");

        }

        public static string GetAPI(string URL, string KeyName)
        {

            string url = ConstConfig.APIURL + "/" + URL;
            if (!string.IsNullOrEmpty(KeyName))
            {

                if (url.IndexOf("?") <= 0 && url.IndexOf("&") <= 0)
                {
                    url += "?KeyName=" + KeyName;
                }
                else
                {
                    url += "&KeyName=" + KeyName;
                }
            }
            return url;
        }
        public bool DownloadFile(string URL, string SaveFileFullName)
        {
            if (File.Exists(SaveFileFullName))
            {
                File.Delete(SaveFileFullName);
            }
            string FilePath = SaveFileFullName.Substring(0, SaveFileFullName.LastIndexOf("\\"));
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using (WebClient client = new WebClient())
            {
               var t= Task.Run(()=> { client.DownloadFile(URL, SaveFileFullName); });
                t.Wait();
            }
            return true;
        }


        public void DownloadFileAsync(string URL, string SaveFileFullName,
    System.Net.DownloadProgressChangedEventHandler client_DownloadProgressChanged,
    AsyncCompletedEventHandler client_DownloadFileCompleted)
        {
            if (File.Exists(SaveFileFullName))
            {
                File.Delete(SaveFileFullName);
            }
            string FilePath = SaveFileFullName.Substring(0, SaveFileFullName.LastIndexOf("\\"));
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(URL), SaveFileFullName);
            }
        }
        





        //public string GetHttpData(string url)
        //{
        //   return Task.Run(() => GetHttpDataAsync(url)).Result;
        //}

        public  string GetHttpData(string Url)
        {
            return GetHttpData(Url, 50000);
        }

        public  string GetHttpData(string Url, int Timeout)
        {
            string sException = null;
            string sRslt = null;
            WebResponse oWebRps = null;
            WebRequest oWebRqst = WebRequest.Create(Url);
            oWebRqst.Timeout = Timeout;
            try
            {
                oWebRps = oWebRqst.GetResponse();
            }
            catch (WebException e)
            {
                sException = e.Message.ToString();
                return sException;
                //EYResponse.Write(sException);
            }
            catch (Exception e)
            {
                sException = e.ToString();
                return sException;
                // EYResponse.Write(sException);
            }
            finally
            {
                if (oWebRps != null)
                {
                    StreamReader oStreamRd = new StreamReader(oWebRps.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    sRslt = oStreamRd.ReadToEnd();
                    oStreamRd.Close();
                    oWebRps.Close();
                }
            }
            return sRslt;
        }

        public  string PostHttpData(string Url, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(Url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                //Response.Write(err);
                return string.Empty;
            }
        }



        /// <summary>
        /// 异步Get数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetHttpDataAsync(string url)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }
        public string PostHttpData(string url, System.Net.Http.HttpContent postdata)
        {
            var t = PostHttpDataAsync(url, postdata);
           // t.Wait();
            return t.Result;
        }

        public async Task<string> PostHttpDataAsync(string url, System.Net.Http.HttpContent postdata)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var response = await httpClient.PostAsync(url, postdata);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();//加await的意思是说，主ＵＩ等待它执行完成后，再继续执行，这种就叫作并行！
            }
            else
            {
                return null;//error
            }
        }

        public T APIJsonDeserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            try
            {
                var obj= JsonConvert.DeserializeObject<ClientAPIJSON<T>>(json);
                if (obj.ErrorCode == 0)
                {
                    return obj.Data;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
            return default(T);
        }
        public ClientAPIJSON<T> APIJsonDeserializeA<T>(string json)
        {
            var result = new ClientAPIJSON<T>();
            result.Data = default(T);
            if (string.IsNullOrEmpty(json))
            {
                return result;
            }

            result = JsonConvert.DeserializeObject<ClientAPIJSON<T>>(json);
            //if (result.ErrorCode != 0)
            //{
            //    result = new ClientAPIJSON<T>();
            //    result.Data = default(T);
            //}
            return result;
        }

        public class ClientAPIJSON<T>
        {
            public int ErrorCode { get; set; }
            public string ErrorMsg { get; set; }
            public T Data { get; set; }

        }

    }
}
