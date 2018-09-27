using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public class MediaHelper
    {
        public  static Task<bool> AmrToMp3Async(string MapPathAMR, string SourceNameOnly)
        {
            return  Task.Run<bool>(()=> AmrToMp3(MapPathAMR, SourceNameOnly));
        }

        /// <summary>
        /// amr转mp3
        /// </summary>
        /// <param name="MapPathAMR">文件绝对路径如:D:\Amr\</param>
        /// <param name="SourceNameOnly">amr文件名，不带前路径，不带后缀</param>
        /// <param name="SaveNameOnly">保存文件名,不带路径，不带后缀，保存在同级目录下</param>
        /// <returns></returns>
        public static bool AmrToMp3(string MapPathAMR, string SourceNameOnly)
        {
            string SaveFullName = MapPathAMR + SourceNameOnly + ".mp3";
            if (File.Exists(SaveFullName))
            {
                File.Delete(SaveFullName);
            }
            if (!File.Exists(MapPathAMR + SourceNameOnly + ".amr"))
            {
                return false;
            }
            string CmdArg = @"-y -i {0}{1}.amr {0}{2}.mp3";
            CmdArg = string.Format(CmdArg, MapPathAMR, SourceNameOnly, SourceNameOnly);
            try
            {
                Process p = new Process();//建立外部调用线程
                p.StartInfo.FileName = ConfigHelper.GetAppendSettingValue("ffmpegFullName");//要调用外部程序的绝对路径
                p.StartInfo.Arguments = CmdArg;//参数(这里就是FFMPEG的参数了)
                p.StartInfo.UseShellExecute = false;//不使用操作系统外壳程序启动线程(一定为FALSE,详细的请看MSDN)
                p.StartInfo.RedirectStandardError = true;//把外部程序错误输出写到StandardError流中(这个一定要注意,FFMPEG的所有输出信息,都为错误输出流,用StandardOutput是捕获不到任何消息的...这是我耗费了2个多月得出来的经验...mencoder就是用standardOutput来捕获的)
                p.StartInfo.CreateNoWindow = false;//不创建进程窗口
                p.Start();//启动线程
                p.WaitForExit();//等待完成
                p.StandardError.ReadToEnd();//开始同步读取
                p.Close();//关闭进程
                p.Dispose();//释放资源

                return true;

            }
            catch (Exception ex)
            {
                //return "error" + ex.Message;
                return false;
            }
        }
    }
}
