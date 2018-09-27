using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography;
//using Xfrog.Net;
//using MCMP.IF.Common;
using System.Collections;
using System.Web.SessionState;
using Tool;
using Newtonsoft.Json;

/// <summary>
///DataHelp 的摘要说明
/// </summary>
public class DataHelper : IRequiresSessionState
{
    public static string GetUseToken()
    {
        string API_Url = ConfigHelper.GetAppendSettingValue(ConfigHelper.CRMConfig.Domain);
        API_Url += "token/get";
        API_Url += "?login_type=2";
        API_Url += "&third_party_type=wx";
        API_Url += "&openid=wx";
        return string.Empty;
    }

    public static string GetWebContent(string Url)
    {

        string strResult = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //声明一个HttpWebRequest请求
            request.Timeout = 50000;
            //设置连接超时时间
            request.Headers.Set("Pragma", "no-cache");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            System.IO.Stream streamReceive = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            strResult = streamReader.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw;
        }
        return strResult;
    }

    //获取当前url 返回数据
    public static string Get_Http(string a_strUrl)
    {
        string strResult = "";
        try
        {
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(a_strUrl);

            myReq.Timeout = 1000;


            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
            Stream myStream = HttpWResp.GetResponseStream();

            StreamReader sr = new StreamReader(myStream, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            while (-1 != sr.Peek())
            {
                strBuilder.Append(sr.ReadLine());
            }

            strResult = strBuilder.ToString();

        }
        catch (Exception ex)
        {
            strResult = "失败:"+ex.Message;
        }
        return strResult;

    }

    public static string GetHttpData(string Url)
    {
        return GetHttpData(Url, 50000);
    }

    public static string GetHttpData(string Url, int Timeout)
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

    public static string PostHttpData(string Url, string postData)
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


    //// 申请默认令牌
    //public static string GetSessionToken()
    //{

    //    try
    //    {
    //         string paras = "user_account=" + userCode + "&user_pwd=" + pwd;
    //         string Token = GetToken("1", paras);
    //         JsonObject jo = new JsonObject(Token);

    //         return jo["token"].ToString().Replace("'","");
    //    }
    //    catch (Exception)
    //    {

    //        return "";
    //    }

    //}



    //获取时间戳
    public static string GetTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString();
    }


    /// <summary>
    /// 获取当前url 返回数据
    /// </summary>
    /// <param name="a_strUrl"></param>
    /// <param name="postparams"></param>
    /// <returns></returns>
    public static string Get_Http_Params(string a_strUrl, string postparams)
    {
        string strResult = "";
        try
        {
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(a_strUrl);
            myReq.Method = "POST";
            myReq.ContentType = "application/json";
            myReq.Headers.Add("Accept-Charset", "utf-8");
            myReq.Headers.Add("Cache-Control", "no-cache");
            byte[] bData = null;
            Encoding encoding = Encoding.GetEncoding("utf-8");
            bData = encoding.GetBytes(postparams);
            myReq.ContentLength = bData.Length;
            Stream newStream = myReq.GetRequestStream();
            newStream.Write(bData, 0, bData.Length);
            newStream.Close();
            myReq.Timeout = 60000;


            HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
            Stream myStream = HttpWResp.GetResponseStream();

            StreamReader sr = new StreamReader(myStream, Encoding.UTF8);

            StringBuilder strBuilder = new StringBuilder();

            while (-1 != sr.Peek())
            {
                strBuilder.Append(sr.ReadLine());
            }

            strResult = strBuilder.ToString();

        }
        catch (Exception ex)
        {

            strResult = "失败";
        }
        return strResult;

    }

    //// 带参申请访问令牌
    //public static string GetToken(string login_type, string paramsStr)
    //{

    //    try
    //    {
    //        string userCode = "";
    //        string pwd = "";
    //        string stamptime = GetTimestamp();
    //        string[] parlist = paramsStr.Split('&');
    //        string third_party_msg = "";
    //        string third_party_type = "";
    //        if (parlist != null)
    //        {
    //            for (int i = 0; i < parlist.Length; i++)
    //            {
    //                if (parlist[i].Split('=')[0].ToString().Equals("user_account"))
    //                {
    //                    userCode = parlist[i].Split('=')[1].ToString();
    //                }
    //                if (parlist[i].Split('=')[0].ToString().Equals("user_pwd"))
    //                {
    //                    pwd = parlist[i].Split('=')[1].ToString();
    //                }
    //                if (parlist[i].Split('=')[0].ToString().Equals("third_party_msg"))
    //                {
    //                    third_party_msg = parlist[i].Split('=')[1].ToString();
    //                }
    //                if (parlist[i].Split('=')[0].ToString().Equals("third_party_type"))
    //                {
    //                    third_party_type = parlist[i].Split('=')[1].ToString();
    //                }

    //            }
    //        }
    //        string Md5_Pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pwd.Trim(), "MD5");
    //        OAuth oauth = new OAuth();
    //        List<Parameter> listParams = new List<Parameter>();
    //        listParams.Add(new Parameter("login_type", login_type));
    //        listParams.Add(new Parameter("user_account", userCode));
    //        listParams.Add(new Parameter("user_pwd", Md5_Pwd));
    //        listParams.Add(new Parameter("third_party_type", third_party_type));
    //        listParams.Add(new Parameter("third_party_msg", third_party_msg));
    //        string url = starturl + "api/token/get";
    //        string querySting = null;
    //        string newUrl = oauth.GetOauthUrl(url, key, appsecret, "", listParams, out querySting);
    //        // 获取页面返回数据
    //        string outData = Get_Http(newUrl);

    //        return outData;
    //    }
    //    catch (Exception)
    //    {

    //        return "";
    //    }

    //}


    ////修改会员信息 
    //public static string Vipmodify(string token, List<MCMP.IF.Common.Parameter> listParams,VipUser user)
    //{
    //    string json = "";
    //    OAuth oauth = new OAuth();
    //    string querySting = null;
    //    string url = starturl + "api/vip/modify";
    //    string newUrl = oauth.GetOauthUrl(url, key, appsecret, token, listParams, out querySting);
    //    json = DataHelp.Get_Http_Params(newUrl, JsonHelper.JsonSerializer(user));


    //    return json;
    //}

    public static string ToJson(object info)
    {
        if (null == info)
        {
            return string.Empty;
        }
        Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
        timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        string Result = JsonConvert.SerializeObject(info, timeConverter);
        return Result;
    }



}