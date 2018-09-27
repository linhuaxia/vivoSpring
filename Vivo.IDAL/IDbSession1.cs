 

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.IDAL
{
	public partial interface IDBSession
    {
	
		IAiQiYiInfoDAL AiQiYiInfoDAL{get;set;}
	
		ILogInfoDAL LogInfoDAL{get;set;}
	
		IPlanInfoDAL PlanInfoDAL{get;set;}
	
		IPrizeResultInfoDAL PrizeResultInfoDAL{get;set;}
	
		IProfilesInfoDAL ProfilesInfoDAL{get;set;}
	
		IRuleInfoDAL RuleInfoDAL{get;set;}
	
		IsysdiagramsDAL sysdiagramsDAL{get;set;}
	
		IUserInfoDAL UserInfoDAL{get;set;}
	
		IWechatMsgInfoDAL WechatMsgInfoDAL{get;set;}
	}	
}