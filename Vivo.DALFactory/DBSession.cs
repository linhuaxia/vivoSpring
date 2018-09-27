using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vivo.IDAL;
using Vivo.Model;
using Vivo.DAL;
using System.Data.Entity;

namespace Vivo.DALFactory
{
    /// <summary>
    /// 数据传话层：就是一个工厂类，
    /// 负责完成所有数据操作类实例的创建，
    /// 然后业务层通过数据传话层来获取要操作的数据库类的实例
    /// 以达到将BLL与DAL解耦
    /// 
    /// 提供一个方法，完成所有数据的保存（一次连数据库，事务提交）
    /// </summary>
    public partial class DBSession:IDBSession
    {
        public DbContext db {
            get {
                return DBContextFactory.CreateDbContext();
            }
        }
        /// <summary>
        /// 统一提交事务方法
        /// 【设计模式:工作单元模式】
        /// </summary>
        /// <returns></returns>
        public bool SaveChanges()
        {
            return db.SaveChanges() > 0;
        }


    }
}
