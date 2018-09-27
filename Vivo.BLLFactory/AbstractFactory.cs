using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vivo.IBLL;


namespace Vivo.BLLFactory
{
    /// <summary>
    /// 抽象工厂，通过反射创建类的实例
    /// </summary>
    public partial class AbstractFactory
    {
        private static readonly string AssemblyPath = ConfigurationManager.AppSettings["BLLAssemblyPath"];
        private static readonly string AssemblyNameSpace = ConfigurationManager.AppSettings["BLLAssemblyNameSpace"];

        private static object CreateInstance(string ClassName)
        {
            var assembly = Assembly.Load(AssemblyPath);
            return assembly.CreateInstance(ClassName);
        }

        //public static IDbHelper CreateDbHelper()
        //{

        //    string fullClassName = AssemblyNameSpace + ".DbHelper";
        //    return CreateInstance(fullClassName) as IDbHelper;

        //}
    }
}
