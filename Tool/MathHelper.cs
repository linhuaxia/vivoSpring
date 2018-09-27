using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
   public static class MathHelper
    {
        /// <summary>
     /// 标准差计算
     /// </summary>
     /// <param name="values"></param>
     /// <returns></returns>
        public static double CalculateStdDev(IEnumerable<double> values)
        {
            if (values.Count()==0)
            {
                return 0;
            }
            return CalculateStdDev(values.Select(a=>Convert.ToDecimal(a)));
        }
        /// <summary>
        /// 标准差计算
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double CalculateStdDev(IEnumerable<decimal> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //  计算平均数   
                var avg = (double)values.Average();
                //  计算各数值与平均数的差值的平方，然后求和 
                var sum = values.Sum(d => Math.Pow((double)d - avg, 2));
                //  除以数量，然后开方
                ret = Math.Sqrt(sum / values.Count());
            }
            return ret;
        }
        /// <summary>
        /// 标准差计算
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double CalculateStdDev(IEnumerable<int> values)
        {
            if (values.Count() == 0)
            {
                return 0;
            }
            return CalculateStdDev(values.Select(a => Convert.ToDecimal(a)));
        }

        ///// <summary>
        ///// 求中位数
        ///// </summary>
        //public static decimal GetMedian(IList<decimal> list)
        //{
        //    if (list.Count()==0)
        //    {
        //        return 0;
        //    }
            
        //    if (list.Count()%2!=0)
        //    {
        //        int MiddleIndex = list.Count() / 2 + 1;
        //        return list[MiddleIndex];
        //    }

        //}
    }
}
