using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Vivo.Model;
using Vivo.IDAL;
using System.Data.Entity;

namespace Vivo.DAL
{
   public class BaseDAL<T> where T:class,new()
    {
       internal readonly DbContext db = DBContextFactory.CreateDbContext();
        public T Create(T info)
        {
            db.Set<T>().Add(info);
            //db.SaveChanges();
            return info;
        }

        public bool Delete(T info)
        {
            db.Entry<T>(info).State = System.Data.Entity.EntityState.Deleted;
            //return db.SaveChanges() > 0;
            return true;
        }
        public bool Delete(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                db.Entry<T>(item).State = System.Data.Entity.EntityState.Deleted;
            }
            //return db.SaveChanges() > 0;
            return true;
        }

        public bool Edit(T info)
        {
            db.Entry<T>(info).State = System.Data.Entity.EntityState.Modified;
            // return db.SaveChanges() > 0;
            return true;
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            return db.Set<T>().Where(whereLambda);
        }

        public IQueryable<T> GetList<s>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, s>> OrderByLambda, bool IsASC)
        {
            var temp = db.Set<T>().Where(whereLambda);
            TotalCount = temp.Count();
            if (IsASC)
            {
                temp = temp.OrderBy(OrderByLambda);
            }
            else
            {
                temp = temp.OrderByDescending(OrderByLambda);
            }
            return temp.Skip((PageIndex - 1) * PageSize).Take(PageSize);
        }

    }
}
