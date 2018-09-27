using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Xml.Xsl;
using System.Text.RegularExpressions;

namespace Tool
{
    /// <summary>
    /// xml操作类
    /// </summary>
    public static class XmlHelper
    {
        private static string RelativePath = HttpContext.Current.Server.MapPath("/Public/xml/");


        public static DataTable CreateDatatable(string FID)
        {
            if (string.IsNullOrEmpty(FID))
            {
                return null;
            }
            string Path = RelativePath + FID + ".xml";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("value", typeof(string));
            ds.ReadXml(Path);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow copyRow in ds.Tables[0].Rows)
                {
                    dt.ImportRow(copyRow);
                }
            }
            return dt;
        }

        /// <summary>
        /// 把title中有“|”的分成两个
        /// </summary>
        /// <param name="XmlTable"></param>
        /// <returns></returns>
        public static DataTable SplitApartXmlTable(DataTable XmlTable)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("title");
            dt.Columns.Add("title2");
            dt.Columns.Add("value");
            DataRow dtRow = dt.NewRow();

            string titles = "";
            foreach (DataRow Ritem in XmlTable.Rows)
            {
                dtRow = dt.NewRow();
                titles = Ritem["title"].ToString();
                if (titles.Split('|').Length==2)
                {
                    dtRow["title"] = titles.Split('|')[0];
                    dtRow["title2"] = titles.Split('|')[1];                    
                }
                else
                    dtRow["title"] = dtRow["title2"] = Ritem["title"].ToString();
                dtRow["value"] = Ritem["value"].ToString();
                dt.Rows.Add(dtRow);
            }
            return dt;            
        }

        /// <summary>
        /// 值是否在指定XML表中存在
        /// </summary>
        /// <param name="FID">xml名。不带后缀，不带路径，默认路径为public/xml</param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsValueIn(string FID, string Value)
        {
            DataTable dt = CreateDatatable(FID);
            foreach (DataRow item in dt.Rows)
            {
                if (item["Value"].ToString()==Value.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 修改XML文件节点，用于修改字典表
        /// </summary>
        /// <param name="FID">指定文件名</param>
        /// <param name="file">文件路径</param>
        /// <param name="ValueList">修改序列</param>
        public static void Edit(string FID, string[] ValueList)
        {
            string file = RelativePath + FID + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode(FID).ChildNodes;

            foreach (string value in ValueList)
            {
                foreach (XmlNode xn in nodeList)   //遍历所有TypeList子节点
                {
                    XmlElement TypeList = (XmlElement)xn;   //将子节点TypeList类型转换为XmlElement类型
                    if (((XmlElement)TypeList.ChildNodes[1]).InnerText == value)
                    {
                        XmlElement title = (XmlElement)TypeList.ChildNodes[0];
                        title.InnerText = HttpContext.Current.Request["title_" + value].ToString();
                        break;
                    }
                }
            }
            xmlDoc.Save(file);//保存。
        }

        /// <summary>
        /// 删除XML文件节点，用于删除字典表
        /// </summary>
        /// <param name="FID">指定文件名</param>
        /// <param name="file">文件路径</param>
        /// <param name="ValueList">删除序列</param>
        public static void Delete(string FID, string[] ValueList)
        {
            string file = RelativePath + FID + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode(FID).ChildNodes;

            foreach (string value in ValueList)
            {
                foreach (XmlNode xn in nodeList)   //遍历所有TypeList子节点
                {
                    XmlElement TypeList = (XmlElement)xn;   //将子节点TypeList类型转换为XmlElement类型
                    if (((XmlElement)TypeList.ChildNodes[1]).InnerText == value)
                    {
                        xn.ParentNode.RemoveChild(xn);
                        break;
                    }
                }
            }
            xmlDoc.Save(file);//保存。
        }

        /// <summary>
        /// 增加XML文件节点，用于增加字典表
        /// </summary>
        /// <param name="FID">指定文件名</param>
        /// <param name="file">文件路径</param>
        public static void Add(string FID, string value, string title)
        {
            string file = RelativePath + FID + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode(FID).ChildNodes;

            XmlElement NTypeList = xmlDoc.CreateElement("TypeList");  //创建一个<TypeList>节点

            XmlElement Ntitle = xmlDoc.CreateElement("title");
            Ntitle.InnerText = title;
            NTypeList.AppendChild(Ntitle);

            XmlElement Nvalue = xmlDoc.CreateElement("value");
            Nvalue.InnerText = value;
            NTypeList.AppendChild(Nvalue);

            if (nodeList.Count == 0)
            {
                xmlDoc.SelectSingleNode(FID).AppendChild(NTypeList);
            }
            else
            {
                int flag = 0;
                foreach (XmlNode xn in nodeList)   //遍历所有TypeList子节点
                {
                    flag++;
                    XmlElement TypeList = (XmlElement)xn;   //将子节点TypeList类型转换为XmlElement类型
                    int oldv = Convert.ToInt32(((XmlElement)TypeList.ChildNodes[1]).InnerText);
                    int newv = Convert.ToInt32(value);
                    if (oldv > newv)
                    {
                        break;
                    }
                }
                if (flag == nodeList.Count)
                {
                    nodeList[flag - 1].ParentNode.InsertAfter(NTypeList, nodeList[flag - 1]);
                }
                else
                {
                    nodeList[flag - 1].ParentNode.InsertBefore(NTypeList, nodeList[flag - 1]);
                }
            }
            xmlDoc.Save(file);//保存。
        }

        public static bool Exist(string FID) {
            return File.Exists(RelativePath + FID + ".xml");
        }

        /// <summary>
        /// 根据value值找出title
        /// </summary>
        /// <param name="filepath">xml名。不带后缀，不带路径，默认路径为public/xml</param>
        /// <param name="Value">value值</param>
        /// <returns></returns>
        public static string GetTitle(string FID, string Value)
        {
            DataTable dt = CreateDatatable(FID);
            foreach (DataRow Rows in dt.Rows)
            {
                if (Rows["value"].ToString() == Value.Trim())
                {
                    return Rows["title"].ToString();
                }
                try
                {
                    if (Rows["value"].ToString() == Value.Trim())
                    { return Rows["title"].ToString(); }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "";
        }

        public static string GetValue(string FID, string Title)
        {
            DataTable dt = CreateDatatable(FID);
            foreach (DataRow Rows in dt.Rows)
            {
                if (Rows["title"].ToString() == Title.Trim())
                {
                    return Rows["value"].ToString();
                }
                try
                {
                    if (Rows["title"].ToString() == Title.Trim())
                    { return Rows["value"].ToString(); }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "";
        }


    }
}
