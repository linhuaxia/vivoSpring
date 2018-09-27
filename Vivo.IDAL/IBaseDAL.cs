using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IDAL
{
    public interface IBaseDAL<T> where T : class, new()
    {
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetList(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="PageIndex">当前第几页</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="TotalCount">输出参数：数据总页码</param>
        /// <param name="whereLambda">搜索条件</param>
        /// <param name="OrderByLambda">泛型方法：排序依据</param>
        /// <param name="IsASC">true：升序，false 降序</param>
        /// <returns></returns>
        IQueryable<T> GetList<s>(int PageIndex, int PageSize, out int TotalCount,
            Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, s>> OrderByLambda,
            bool IsASC);
        bool Delete(T info);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool Delete(IEnumerable<T> list);
        bool Edit(T info);
        T Create(T info);

    }
}
