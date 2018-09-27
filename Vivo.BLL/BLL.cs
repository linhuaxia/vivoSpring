 
using Vivo.IBLL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.BLL
{
	
	public partial class AiQiYiInfoService :BaseService<AiQiYiInfo>,IAiQiYiInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.AiQiYiInfoDAL;
        }
    }   
	
	public partial class LogInfoService :BaseService<LogInfo>,ILogInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.LogInfoDAL;
        }
    }   
	
	public partial class PlanInfoService :BaseService<PlanInfo>,IPlanInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.PlanInfoDAL;
        }
    }   
	
	public partial class PrizeResultInfoService :BaseService<PrizeResultInfo>,IPrizeResultInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.PrizeResultInfoDAL;
        }
    }   
	
	public partial class ProfilesInfoService :BaseService<ProfilesInfo>,IProfilesInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.ProfilesInfoDAL;
        }
    }   
	
	public partial class RuleInfoService :BaseService<RuleInfo>,IRuleInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.RuleInfoDAL;
        }
    }   
	
	public partial class sysdiagramsService :BaseService<sysdiagrams>,IsysdiagramsService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.sysdiagramsDAL;
        }
    }   
	
	public partial class UserInfoService :BaseService<UserInfo>,IUserInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.UserInfoDAL;
        }
    }   
	
	public partial class WechatMsgInfoService :BaseService<WechatMsgInfo>,IWechatMsgInfoService
    {
    

		 public override void SetCurrentDAL()
        {
            CurrentDAL = this.CurrentDBSession.WechatMsgInfoDAL;
        }
    }   
	
}