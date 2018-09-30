using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Vivo.Model;
using System.Runtime.Remoting.Messaging;

namespace Vivo.DAL
{
    /// <summary>
    /// 负责创建EF实例，并保证其线程内唯一
    /// （//TODO:为什么不用单例？线程内唯一能解决并发修改数据问题？)
    /// </summary>
   public class DBContextFactory
    {
        private const string DbContextName = "dbContext";
        public static DbContext CreateDbContext()
        {
            DbContext dbContext = (DbContext)CallContext.GetData(DbContextName);
            if (null== dbContext)
            {
                dbContext = new vivoIndiaEntities();
                CallContext.SetData(DbContextName, dbContext);
            }
            return dbContext;
        }
    }
}
