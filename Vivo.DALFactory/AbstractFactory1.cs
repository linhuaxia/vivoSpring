 

using Vivo.IDAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.DALFactory
{
    public partial class AbstractFactory
    {
      
   
		
	    public static IAiQiYiInfoDAL CreateAiQiYiInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".AiQiYiInfoDAL";
          return CreateInstance(fullClassName) as IAiQiYiInfoDAL;

        }
		
	    public static ILogInfoDAL CreateLogInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".LogInfoDAL";
          return CreateInstance(fullClassName) as ILogInfoDAL;

        }
		
	    public static IPlanInfoDAL CreatePlanInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".PlanInfoDAL";
          return CreateInstance(fullClassName) as IPlanInfoDAL;

        }
		
	    public static IPrizeResultInfoDAL CreatePrizeResultInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".PrizeResultInfoDAL";
          return CreateInstance(fullClassName) as IPrizeResultInfoDAL;

        }
		
	    public static IProfilesInfoDAL CreateProfilesInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".ProfilesInfoDAL";
          return CreateInstance(fullClassName) as IProfilesInfoDAL;

        }
		
	    public static IRuleInfoDAL CreateRuleInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".RuleInfoDAL";
          return CreateInstance(fullClassName) as IRuleInfoDAL;

        }
		
	    public static IsysdiagramsDAL CreatesysdiagramsDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".sysdiagramsDAL";
          return CreateInstance(fullClassName) as IsysdiagramsDAL;

        }
		
	    public static IUserInfoDAL CreateUserInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".UserInfoDAL";
          return CreateInstance(fullClassName) as IUserInfoDAL;

        }
		
	    public static IWechatMsgInfoDAL CreateWechatMsgInfoDAL()
        {

		 string fullClassName = AssemblyNameSpace + ".WechatMsgInfoDAL";
          return CreateInstance(fullClassName) as IWechatMsgInfoDAL;

        }
	}
	
}