using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IBLL
{
   public partial interface IProfilesInfoService : IBaseService<ProfilesInfo>
    {
        /// <summary>
        /// 获取ProfilesInfo
        /// </summary>
        /// <param name="id">id</param>
        ProfilesInfo Get(string Keys, bool WithCreate);

        /// <summary>
        /// 获取ProfilesInfo的Content
        /// </summary>
        /// <param name="id">id</param>
        string GetValue(string keys, bool WithCreate);

        /// <summary>
        /// 获取ProfilesInfo
        /// </summary>
        /// <param name="id">id</param>
        string GetValue(string keys);
        ProfilesInfo Get(string Keys);


    }
}
