using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tool;

namespace Tool
{
    /// <summary>
    /// 使用此类前需先在config文件中配置服务器ApiUrl
    /// </summary>
    public class APIHellper
    {
        public static class ConstConfig
        {
            /// <summary>
            /// api服务器地址
            /// </summary>
            public static readonly string APIURL = "http://api.yunchengtech.com/api";

        }

        public static class API
        {
            public static class CustomerKeyLockRecord
            {
                ///// <summary>
                ///// 根据锁号获取最新一个解锁信息
                ///// </summary>
                ///// <param name="KeyName"></param>
                ///// <returns></returns>
                //public async static string GetLastByKeyName(string KeyName)
                //{
                //    string URL = "CustomerKeyLockRecord/GetLastByKeyName";
                //    URL = APIHellper.GetAPI(URL, KeyName);
                //    string TaskJson = await new APIHellper().PostHttpData(URL,null);
                //    return TaskJson;

                //}
            }
        }

        public static string GetAPI(string URL, string KyeName)
        {

            string url = ConstConfig.APIURL + "/" + URL;
            if (!string.IsNullOrEmpty(KyeName))
            {

                if (url.IndexOf("?") <= 0 && url.IndexOf("&")<=0)
                {
                    url += "?KeyName=" + KyeName;
                }
                else
                {
                    url += "&KeyName=" + KyeName;
                }
            }
            return url;
        }

        public void DownloadFileAsync(string URL, string SaveFileFullName,
            System.Net.DownloadProgressChangedEventHandler client_DownloadProgressChanged,
            AsyncCompletedEventHandler client_DownloadFileCompleted)
        {
            Console.WriteLine("下载文件方法有进来");
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
                client.DownloadFileAsync(new Uri(URL), SaveFileFullName);
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
            }
        }

        public void DownloadFileAsync(string URL, string SaveFileFullName)
        {
            DownloadFileAsync(URL, SaveFileFullName, null, null);
        }


        public async Task<Stream> GetResponseStream(string URL)
        {
            return await GetResponseStreamAsync(URL);
        }
        public async Task<Stream> GetResponseStreamAsync(string URL)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var response = await httpClient.GetAsync(URL);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            return null;
        }


        /// <summary>
        /// 异步Get数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetHttpData(string url)
        {
            return await GetHttpDataAsync(url);
        }
        /// <summary>
        /// 异步Get数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetHttpDataAsync(string url)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();//加await的意思是说，主ＵＩ等待它执行完成后，再继续执行，这种就叫作并行！
            }
            return null;
        }
        public async Task<string> PostHttpData(string url, System.Net.Http.HttpContent postdata)
        {
            return await PostHttpDataAsync(url, postdata);
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

            var result = JsonConvert.DeserializeObject<ClientAPIJSON<T>>(json);
            if (result.ErrorCode != 0)
            {
                return default(T);
            }
            return result.Data;
        }

        public class ClientAPIJSON<T>
        {
            public int ErrorCode { get; set; }
            public string ErrorMsg { get; set; }
            public T Data { get; set; }

        }

    }
}
