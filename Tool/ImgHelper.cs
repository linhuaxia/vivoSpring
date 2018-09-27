using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;


namespace Tool
{
    public class ImgHelper
    {
        public const string TempDir = "/Content/upload/TempUpload/";
        public const string ImageDir = "/Content/upload/Img/";
        /// <summary>
        /// 把图片放入临时文件夹，并返回文件名
        /// </summary>
        /// <param name="TargetFile"></param>
        /// <param name="c"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static string CheckUpload(HttpPostedFile TargetFile, Control c, Type ty)
        {
            if (string.IsNullOrEmpty(TargetFile.FileName) || 0 == TargetFile.ContentLength)
            {
                ScriptManager.RegisterClientScriptBlock(c, ty, "click", "alert('请上传图片！');", true);
                return string.Empty;
            }
            string fileExtension = System.IO.Path.GetExtension(TargetFile.FileName).ToLower();
            bool FileOk = false;
            string[] allowFiles = { "image/pjpeg", "image/bmp", "image/gif", "", "image/png" };
            if (TargetFile.ContentLength > 524288)//图片大小
            {
                FileOk = false;
                ScriptManager.RegisterClientScriptBlock(c, ty, "click", "alert('上传的图片不能超过512KB！');", true);
                return string.Empty;
            }
            foreach (string allowfile in allowFiles)//文件格式
            {
                if (TargetFile.ContentType == allowfile) { FileOk = true; }
            }
            if (FileOk)
            {
                Random rand = new Random();
                string FileName = DateTime.Now.ToString("yyyy_MM") + "/" + DateTime.Now.ToString("yyyyMMddhhmmss") + rand.Next(10000, 99999) + fileExtension;
                string ServerPath = Path.GetDirectoryName(System.Web.HttpContext.Current.Server.MapPath(TempDir + "") + FileName);
                if (!Directory.Exists(ServerPath))
                {
                    Directory.CreateDirectory(ServerPath);
                }
                TargetFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath(TempDir + "") + FileName);
                return FileName;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(c, ty, "click", "alert('只能上传jpg,bmp,gif,png格式图片！');", true);
                return string.Empty;
            }
        }

        /// <summary>
        /// 把图片移到正式文件夹
        /// </summary>
        /// <param name="FlieName"></param>
        /// <returns></returns>
        public static bool RemovePathAndADelSource(string FlieName)
        {
            string OldPath = System.Web.HttpContext.Current.Server.MapPath("~" + TempDir);
            string OldFile = (OldPath + FlieName).Replace("\\", "/");
            string NewFile = OldFile.Replace(TempDir, ImageDir).Replace("\\", "/");;
            string NewPath = NewFile.Substring(0, NewFile.LastIndexOf("/"));

            if (File.Exists(OldFile))
            {
                if (!Directory.Exists(NewPath))
                {
                    Directory.CreateDirectory(NewPath);
                }
                File.Copy(OldFile, NewFile);
                File.Delete(OldFile);
                return true;
            } return false;
        }

        /// <summary>
        /// 保存KindEditor时的图片移动（返回内容是新图片路径）
        /// </summary>
        /// <param name="KindEditorString"></param>
        /// <returns>图片转换后KindEditor的Text</returns>
        public static string SaveKindEditorAndImg(string KindEditorString)
        {//<img border="0" alt="" src="/TempUpload/20101126095119_0781.gif" />
            Regex re = new Regex(@"<img[\s\S]*?src=""" + TempDir + @"([\s\S]*?)""", RegexOptions.IgnoreCase);
            MatchCollection TheMatches = re.Matches(KindEditorString);
            foreach (Match nextmatch in TheMatches)
            {
                if (!string.IsNullOrEmpty(nextmatch.Groups[1].ToString().Trim()))
                { RemovePathAndADelSource(nextmatch.Groups[1].ToString().Trim()); }
            }
            return Function.ReplaceText(KindEditorString, TempDir.Trim('/'), ImageDir.Trim('/'));

        }

        ///// <summary>
        ///// 文件移位并正则替换路径名
        ///// </summary>
        ///// <param name="OldString"></param>
        ///// <returns></returns>
        //private static string ReplacePath(string OldString)
        //{
        //    Regex re = new Regex(@"<IMG[\s\S]*?src=""[\s\S]*?"+TempDir+@"([\s\S]*?)""", RegexOptions.IgnoreCase);
        //    MatchCollection TheMatches = re.Matches(OldString);
        //    foreach (Match nextmatch in TheMatches)
        //    {
        //        if (!string.IsNullOrEmpty(nextmatch.Groups[1].ToString().Trim()))
        //        { RemovePathAndADelSource(nextmatch.Groups[1].ToString().Trim()); }
        //    }
        //    re = new Regex(@"<IMG[\s\S]*?src=""([\s\S]*?)"+TempDir+@"[\s\S]*?""", RegexOptions.IgnoreCase);
        //    OldString = Tool.ReplaceText(OldString, "http://www.22220008.com", "");

        //    return Tool.ReplaceText(OldString, @"IMGTempInsert", "IMG");
        //}

        private static bool ThumbnailCallback()
        {
            return false;
        }

        /// <param name="SavePath">保存的相对路径</param>
        /// <param name="picFilePath">原图相对路径</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public static void GetThumbnailImage(string SavePath, string picFilePath, int width, int height)
        {
            //添加small100的前缀大小
            //程序内相对的服务器路径小图片

            string SmallPath100 = HttpContext.Current.Server.MapPath(SavePath);
            string machpath = HttpContext.Current.Server.MapPath(picFilePath);
            string isDir = SmallPath100.Substring(0, SmallPath100.LastIndexOf('\\'));
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(isDir);
            if (!di.Exists)
            {
                di.Create();
            }
            Bitmap img = new Bitmap(machpath);  //read picture to memory

            int h = img.Height;
            int w = img.Width;
            int ss, os;// source side and objective side
            double temp1, temp2;
            //compute the picture's proportion
            temp1 = (h * 1.0D) / height;
            temp2 = (w * 1.0D) / width;
            if (temp1 < temp2)
            {
                ss = w;
                os = width;
            }
            else
            {
                ss = h;
                os = height;
            }

            double per = (os * 1.0D) / ss;
            if (per < 1.0D)
            {
                h = (int)(h * per);
                w = (int)(w * per);
            }
            // create the thumbnail image
            System.Drawing.Image imag2 = img.GetThumbnailImage(w, h,
                new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback),
                IntPtr.Zero);
            Bitmap tempBitmap = new Bitmap(w, h);
            System.Drawing.Image tempImg = System.Drawing.Image.FromHbitmap(tempBitmap.GetHbitmap());
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tempImg);
            g.Clear(Color.White);

            int x, y;

            x = (tempImg.Width - imag2.Width) / 2;
            y = (tempImg.Height - imag2.Height) / 2;

            g.DrawImage(imag2, x, y, imag2.Width, imag2.Height);

            try
            {
                if (img != null)
                    img.Dispose();
                if (imag2 != null)
                    imag2.Dispose();
                if (tempBitmap != null)
                    tempBitmap.Dispose();

                string fileExtension = System.IO.Path.GetExtension(machpath).ToLower();
                //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                switch (fileExtension)
                {
                    case ".gif": tempImg.Save(SmallPath100, ImageFormat.Gif); break;
                    case ".jpg": tempImg.Save(SmallPath100, ImageFormat.Jpeg); break;
                    case ".bmp": tempImg.Save(SmallPath100, ImageFormat.Bmp); break;
                    case ".png": tempImg.Save(SmallPath100, ImageFormat.Png); break;
                }
            }
            catch
            {
                throw new Exception("图片上传失败");
            }
            finally
            {
                //释放内存
                if (tempImg != null)
                    tempImg.Dispose();
                if (g != null)
                    g.Dispose();
            }
        }
    }

}