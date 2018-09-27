using Vivo.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IBLL
{
    public interface IBaseService<T> where T : class, new() 
    {
        IDBSession CurrentDBSession { get; }
        IDAL.IBaseDAL<T> CurrentDAL { get; set; }
        T Create(T info);
        bool Delete(T info);
        bool Edit(T info);
        IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> GetList<s>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, s>> OrderByLambda, bool IsASC);
    }
}
