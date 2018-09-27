using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using KinnSoft.Excel.OfficeOpenXml;//ʹ��Excel2007����DLL

namespace Tool
{
    /// <summary>
    ///  Output <see cref="DataTable"/> to Excel2007 or above version that base on open xml formater
    /// </summary>
    public class NPOIHelper
    {
        public const int MaxSheetRows2007 = 1048576;
        public static void Export(DataTable table, string fileName)
        {
            int rows = 0;
            Export(table, fileName, string.Empty, ref rows);
        }
        public static void Export(DataTable table, string fileName, string sheetName, ref int rowWrited)
        {
            if (table == null || table.Rows.Count == 0)
                return;
            if (string.IsNullOrEmpty(sheetName))
                sheetName = "Sheet1";
            //if (table.Rows.Count > ExcelUtil.GetMaxRowSupported(fileName))
            //    throw new ArgumentException(string.Format("data rows cann't be more than {0}",
            //        ExcelUtil.GetMaxRowSupported(fileName)));
            var excel = new ExcelPackage(new FileInfo(fileName));
            WriteSheets(table, excel, sheetName, ref rowWrited);
        }
        public static void Export(DataTable table, string fileName, string sheetName, int BeginRowIndex)
        {
            if (table == null || table.Rows.Count == 0)
                return;
            if (string.IsNullOrEmpty(sheetName))
                sheetName = "Sheet1";
            //if (table.Rows.Count > ExcelUtil.GetMaxRowSupported(fileName))
            //    throw new ArgumentException(string.Format("data rows cann't be more than {0}",
            //        ExcelUtil.GetMaxRowSupported(fileName)));
            var excel = new ExcelPackage(new FileInfo(fileName));
            WriteSheets(table, excel, sheetName, BeginRowIndex);
        }

        private static MemoryStream Write(DataTable table, string sheetName)
        {
            var ms = new MemoryStream();
            int rowWrited = 0;
            var excel = new ExcelPackage(ms);
            WriteSheets(table, excel, sheetName, ref rowWrited);
            return ms;
        }


        private static void WriteSheets(DataTable table, ExcelPackage excel, string sheetName, ref int rowWrited)
        {
            using (excel)
            {
                int max = MaxSheetRows2007 - 1;
                int rows = table.Rows.Count;
                int sheetCount = (rows % max == 0) ? rows / max : rows / max + 1;
                rowWrited = 0;
                for (int sheetNo = 0; sheetNo < sheetCount; sheetNo++)
                {
                    WriteSheet(table, excel, (sheetNo == 0) ? sheetName : sheetName + "_" + sheetNo,
                               sheetNo * max, (sheetNo + 1) * max < rows ? (sheetNo + 1) * max - 1 : rows - 1,
                               ref rowWrited);
                }
                //WriteSheet(table, sheetIndex, ref rowWrited);
                excel.Save();
                excel.Close();
            }
        }

        private static void WriteSheets(DataTable table, ExcelPackage excel, string sheetName,  int BeginRowIndex)
        {
            using (excel)
            {
                int max = MaxSheetRows2007 - 1;
                int rows = table.Rows.Count;
                int sheetCount = (rows % max == 0) ? rows / max : rows / max + 1;
               int rowWrited = 0;
               for (int sheetNo = 0; sheetNo < sheetCount; sheetNo++)
                {
                    MyWriteSheet(table, excel, (sheetNo == 0) ? sheetName : sheetName + "_" + sheetNo,
                               BeginRowIndex, (sheetNo + 1) * max < rows ? (sheetNo + 1) * max - 1 : rows - 1,
                               ref rowWrited);
                }
                //WriteSheet(table, sheetIndex, ref rowWrited);
                excel.Save();
                excel.Close();
            }
        }

        private static ExcelWorksheet CreateSheet(ExcelPackage excel, string sheetName)
        {
            foreach (ExcelWorksheet sheet in
                excel.Workbook.Worksheets.Cast<ExcelWorksheet>().Where(sheet => string.Compare(sheet.Name, sheetName, true) == 0))
            {
                return sheet;
            }
            return excel.Workbook.Worksheets.Add(sheetName);
        }

        /// <summary>
        /// 自己加的备注，未必准确
        /// </summary>
        /// <param name="table">源表</param>
        /// <param name="excel">ExcelPackage</param>
        /// <param name="sheetName">要写入的工开簿名</param>
        /// <param name="startRowIndex">由第几行开始写</param>
        /// <param name="endRowIndex">写到第几行结束</param>
        /// <param name="rowWrited">输出参数，未知其意思</param>
        private static void MyWriteSheet(DataTable table, ExcelPackage excel, string sheetName, int startRowIndex, int endRowIndex,
                             ref int rowWrited)
        {
            var sheet = CreateSheet(excel, sheetName);
            int ColumnIndexFlag = 1;
            foreach (DataColumn col in table.Columns)
            {
                sheet.Rows[startRowIndex].Cells[ColumnIndexFlag].Value = col.ColumnName;
                ColumnIndexFlag++;
            }
           int RowIndex = startRowIndex;
           foreach (DataRow item in table.Rows)
           {
               for (int ColumnIndex = 1; ColumnIndex < ColumnIndexFlag; ColumnIndex++)
               {
                   sheet.Rows[RowIndex].Cells[ColumnIndex].Value = item[ColumnIndex - 1].ToString();
               }
               RowIndex++;
           }
        }



        /// <summary>
        /// 自己加的备注，未必准确
        /// </summary>
        /// <param name="table">源表</param>
        /// <param name="excel">ExcelPackage</param>
        /// <param name="sheetName">要写入的工开簿名</param>
        /// <param name="startRowIndex">由第几行开始写</param>
        /// <param name="endRowIndex">写到第几行结束</param>
        /// <param name="rowWrited">输出参数，未知其意思</param>
        private static void WriteSheet(DataTable table, ExcelPackage excel, string sheetName, int startRowIndex, int endRowIndex,
                             ref int rowWrited)
        {
            var sheet = CreateSheet(excel, sheetName);
            int i = 1;
            foreach (DataColumn col in table.Columns)
            {
                sheet.Rows[1].Cells[i].Value = col.ColumnName;
                i++;
            }
            int columnCount = table.Columns.Count;
            i = 2;
            for (int m = startRowIndex; m <= endRowIndex; m++)
            {
                DataRow row = table.Rows[m];
                for (int j = 1; j <= columnCount; j++)
                {
                    sheet.Rows[i].Cells[j].Value = row[j - 1].ToString();
                }
                i++;
                rowWrited++;
            }
        }

        public static DataTable Import(string fileName)
        {
            var dt = new DataTable();
            using (var excel = new ExcelPackage(new FileInfo(fileName)))
            {
                var sheet = excel.Workbook.Worksheets[1];
                foreach (ExcelCell cell in sheet.Rows[1].Cells)
                {
                    dt.Columns.Add(cell.Value);
                }
                int i = 1;
                foreach (ExcelRow row in sheet.Rows)
                {
                    if (i == 1)
                    {
                        i++;
                        continue;
                    }
                    var values = new List<string>();

                    // * ע�͵Ĵ���ᵼ�µ���ʱ����п�ֵ��������е�ֵ����ǰ�졣
                    //foreach (ExcelCell cell in row.Cells)
                    //{
                    //    values.Add(cell.Value);
                    //}

                    for (int index = 1; index <= dt.Columns.Count; index++)
                    {
                        if (row.Cells.ContainsKey(index))
                        {
                            values.Add(row.Cells[index].Value);
                        }
                        else
                        {
                            values.Add("");
                        }
                    }

                    dt.Rows.Add(values.ToArray());
                    i++;
                }
            }
            return dt;
        }

        public static DataTable Import(string fileName, string sheetName)
        {
            var dt = new DataTable();
            using (var excel = new ExcelPackage(new FileInfo(fileName)))
            {
                var sheet = excel.Workbook.Worksheets[sheetName];
                foreach (ExcelCell cell in sheet.Rows[1].Cells)
                {
                    dt.Columns.Add(cell.Value);
                }
                int i = 1;
                foreach (ExcelRow row in sheet.Rows)
                {
                    if (i == 1)
                    {
                        i++;
                        continue;
                    }
                    var values = new List<string>();
                    for (int index = 1; index <= dt.Columns.Count; index++)
                    {
                        if (row.Cells.ContainsKey(index))
                        {
                            values.Add(row.Cells[index].Value);
                        }
                        else
                        {
                            values.Add("");
                        }
                    }
                    dt.Rows.Add(values.ToArray());
                    i++;
                }
            }
            return dt;
        }
    }
}
