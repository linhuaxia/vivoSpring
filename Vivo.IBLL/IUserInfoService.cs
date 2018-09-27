using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IBLL
{
   public partial interface IUserInfoService : IBaseService<UserInfo>
    {
        UserInfo GetCurrent();


        void Logout();

        void SetUserInfo(int RemberHour, UserInfo info);

    }
}
