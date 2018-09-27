using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IDAL
{
    /// <summary>
    /// 业务层调用的是数据传话层接口
    /// </summary>
    public partial interface IDBSession
    {
        DbContext db { get; }
        bool SaveChanges();
    }
}
