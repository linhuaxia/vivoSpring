using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vivo.IDAL;
using Vivo.DALFactory;
using System.Linq.Expressions;

namespace Vivo.BLL
{
    public abstract class BaseService<T> where T : class, new()
    {
        public BaseService()
        {
            SetCurrentDAL();
        }

        public IDAL.IBaseDAL<T> CurrentDAL { get; set; }
        public abstract void SetCurrentDAL();

        public IDBSession CurrentDBSession
        {
            get {
                // return new DBSession();
                return DBSessionFactory.CreateDBSession();
            }
        }



        public T Create(T info)
        {
            CurrentDAL.Create(info);
            CurrentDBSession.SaveChanges();
            return info;
        }

        public bool Delete(T info)
        {
            CurrentDAL.Delete(info);
            return CurrentDBSession.SaveChanges();
        }

        public bool Edit(T info)
        {
            CurrentDAL.Edit(info);
            return CurrentDBSession.SaveChanges();
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda)
        {
            return CurrentDAL.GetList(whereLambda);
        }

        public IQueryable<T> GetList<s>(int PageIndex, int PageSize, out int TotalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, s>> OrderByLambda, bool IsASC)
        {
            return CurrentDAL.GetList<s>(PageIndex, PageSize, out TotalCount, whereLambda, OrderByLambda, IsASC);
            
        }


    }
}
