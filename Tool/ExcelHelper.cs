using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace Tool
{
    public static class ExcelHelper
    {
        private const string ExcelConnection8 = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source={0};Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
        #region --读取Excel填充到DS ExcelToDS(string filenameurl, string tablename)

        /// <summary>
        /// 获取所有工作簿名
        /// </summary>
        /// <param name="FileNameURL"></param>
        /// <returns></returns>
        public static List<string> GetExcelTableName(string FileNameURL)
        {
            List<string> result = new List<string>();
            string strConn = string.Format(ExcelConnection8, FileNameURL);
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow item in dt.Rows)
                {
                    result.Add(item[2].ToString());
                }
                conn.Close();
                GC.Collect();
            }
            return result;
        }


        /// <summary>
        /// 读取Excel填充到DS
        /// </summary>
        /// <param name="filenameurl"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DataTable ExcelToDatatable(string filenameurl, string tablename)
        {
            if (string.IsNullOrEmpty(tablename)) tablename = "[Sheet1$]";
            string strConn = string.Format(ExcelConnection8, filenameurl);
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();
                DataSet ds = new DataSet();
                OleDbDataAdapter odda = new OleDbDataAdapter("select * from [" + tablename+"] ", conn);
                odda.Fill(ds, tablename);
                conn.Close();//原来的竟然没关闭。。。
                GC.Collect();
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                { process.Kill(); }
                return ds.Tables[0];
            }

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">要导出的表</param>
        /// <param name="FileFullName">保存到完整物理路径</param>
        public static void DataTableToCSV(DataTable dt,string FileFullPath,string FileName)
        {
            if (!Directory.Exists(FileFullPath))
            {
                Directory.CreateDirectory(FileFullPath);
            }
            System.IO.FileStream fs = new FileStream(FileFullPath+FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding());
            //Tabel header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");
            //Table body
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(DelQuota(dt.Rows[i][j].ToString()));
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
        }

        /// <summary>
        /// Delete special symbol
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DelQuota(string str)
        {
            string result = str;
            string[] strQuota = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "`", ";", "'", ",", ".", "/", ":", "/,", "<", ">", "?" };
            for (int i = 0; i < strQuota.Length; i++)
            {
                if (result.IndexOf(strQuota[i]) > -1)
                    result = result.Replace(strQuota[i], "");
            }
            return result;
        }

    }
}
