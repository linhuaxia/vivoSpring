 

using Vivo.IBLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.BLLFactory
{
    public partial class AbstractFactory
    {
      
   
		
	    public static IAiQiYiInfoService CreateAiQiYiInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".AiQiYiInfoService";
			return CreateInstance(fullClassName) as IAiQiYiInfoService;

        }
		
	    public static ILogInfoService CreateLogInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".LogInfoService";
			return CreateInstance(fullClassName) as ILogInfoService;

        }
		
	    public static IPlanInfoService CreatePlanInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".PlanInfoService";
			return CreateInstance(fullClassName) as IPlanInfoService;

        }
		
	    public static IPrizeResultInfoService CreatePrizeResultInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".PrizeResultInfoService";
			return CreateInstance(fullClassName) as IPrizeResultInfoService;

        }
		
	    public static IProfilesInfoService CreateProfilesInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".ProfilesInfoService";
			return CreateInstance(fullClassName) as IProfilesInfoService;

        }
		
	    public static IRuleInfoService CreateRuleInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".RuleInfoService";
			return CreateInstance(fullClassName) as IRuleInfoService;

        }
		
	    public static IsysdiagramsService CreatesysdiagramsService()
        {

			string fullClassName = AssemblyNameSpace + ".sysdiagramsService";
			return CreateInstance(fullClassName) as IsysdiagramsService;

        }
		
	    public static IUserInfoService CreateUserInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".UserInfoService";
			return CreateInstance(fullClassName) as IUserInfoService;

        }
		
	    public static IWechatMsgInfoService CreateWechatMsgInfoService()
        {

			string fullClassName = AssemblyNameSpace + ".WechatMsgInfoService";
			return CreateInstance(fullClassName) as IWechatMsgInfoService;

        }
	}
	
}