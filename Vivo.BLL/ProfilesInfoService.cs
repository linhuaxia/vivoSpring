using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vivo.Model;
using Vivo.IDAL;
using Vivo.IBLL;
using Tool;

namespace Vivo.BLL
{
    public partial class ProfilesInfoService : BaseService<ProfilesInfo>, IProfilesInfoService
    {
        /// <summary>
        /// 获取ProfilesInfo
        /// </summary>
        /// <param name="id">id</param>
        public ProfilesInfo Get(string Keys, bool WithCreate)
        {
            ProfilesInfo info =CurrentDAL.GetList(p=>p.Key==Keys).FirstOrDefault() ;
            if (null == info && WithCreate)
            {
                info = new ProfilesInfo();
                info.Key = Keys;
                info.Value = string.Empty;
                Create(info);
            }
            return info;
        }

        /// <summary>
        /// 获取ProfilesInfo的Content
        /// </summary>
        /// <param name="id">id</param>
        public  string GetValue(string keys, bool WithCreate)
        {
            ProfilesInfo info = Get(keys, WithCreate);
            return info == null ? string.Empty : info.Value;
        }

        /// <summary>
        /// 获取ProfilesInfo
        /// </summary>
        /// <param name="id">id</param>
        public  string GetValue(string keys)
        {
            return GetValue(keys, false);
        }
        public  ProfilesInfo Get(string Keys)
        {
            return Get(Keys, false);
        }
    }
}
 