using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Tool
{
   public static class PdfHelper
    {
        
        /// <summary>
        /// HTML生成PDF
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="path">PDF存放路径</param>
        public static bool HtmlToPdf(string url, string path)
        {
            string wkhtmltopdfToolFullName = HttpContext.Current.Server.MapPath("~/bin/wkhtmltopdf.exe");
            return HtmlToPdf(wkhtmltopdfToolFullName, url, path);
        }

        public static bool HtmlToPdf(string wkhtmltopdfToolFullName, string url, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
                    return false;

                url = url.Trim('"');
                url = "\"" + url + "\"";

                Process p = new Process();
                if (!System.IO.File.Exists(wkhtmltopdfToolFullName))
                    return false;
                string switches = "--print-media-type ";
                switches += "--margin-top 0mm --margin-bottom 4mm --margin-right 0mm --margin-left 0mm ";
                switches += "--page-size A4 ";
                // switches += "--no-background ";
                switches += "--redirect-delay 100%";
                //p.SessionId == "1";
                p.StartInfo.FileName = wkhtmltopdfToolFullName;
                p.StartInfo.Arguments = switches + " " + url + " " + path;
                //p.StartInfo.Arguments = " \"" + url + "\" " + path;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                System.Threading.Thread.Sleep(800);
                // p.SessionId
                p.WaitForExit();
                p.Close();
                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex);
            }
            return false;
        }
    }
}
