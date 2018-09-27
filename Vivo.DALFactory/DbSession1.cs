 
using Vivo.DAL;
using Vivo.IDAL;
using Vivo.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivo.DALFactory
{
	public partial class DBSession : IDBSession
    {
	
		private IAiQiYiInfoDAL _AiQiYiInfoDAL;
        public IAiQiYiInfoDAL AiQiYiInfoDAL
        {
            get
            {
                if(_AiQiYiInfoDAL == null)
                {
                    _AiQiYiInfoDAL = AbstractFactory.CreateAiQiYiInfoDAL();
                }
                return _AiQiYiInfoDAL;
            }
            set { _AiQiYiInfoDAL = value; }
        }
	
		private ILogInfoDAL _LogInfoDAL;
        public ILogInfoDAL LogInfoDAL
        {
            get
            {
                if(_LogInfoDAL == null)
                {
                    _LogInfoDAL = AbstractFactory.CreateLogInfoDAL();
                }
                return _LogInfoDAL;
            }
            set { _LogInfoDAL = value; }
        }
	
		private IPlanInfoDAL _PlanInfoDAL;
        public IPlanInfoDAL PlanInfoDAL
        {
            get
            {
                if(_PlanInfoDAL == null)
                {
                    _PlanInfoDAL = AbstractFactory.CreatePlanInfoDAL();
                }
                return _PlanInfoDAL;
            }
            set { _PlanInfoDAL = value; }
        }
	
		private IPrizeResultInfoDAL _PrizeResultInfoDAL;
        public IPrizeResultInfoDAL PrizeResultInfoDAL
        {
            get
            {
                if(_PrizeResultInfoDAL == null)
                {
                    _PrizeResultInfoDAL = AbstractFactory.CreatePrizeResultInfoDAL();
                }
                return _PrizeResultInfoDAL;
            }
            set { _PrizeResultInfoDAL = value; }
        }
	
		private IProfilesInfoDAL _ProfilesInfoDAL;
        public IProfilesInfoDAL ProfilesInfoDAL
        {
            get
            {
                if(_ProfilesInfoDAL == null)
                {
                    _ProfilesInfoDAL = AbstractFactory.CreateProfilesInfoDAL();
                }
                return _ProfilesInfoDAL;
            }
            set { _ProfilesInfoDAL = value; }
        }
	
		private IRuleInfoDAL _RuleInfoDAL;
        public IRuleInfoDAL RuleInfoDAL
        {
            get
            {
                if(_RuleInfoDAL == null)
                {
                    _RuleInfoDAL = AbstractFactory.CreateRuleInfoDAL();
                }
                return _RuleInfoDAL;
            }
            set { _RuleInfoDAL = value; }
        }
	
		private IsysdiagramsDAL _sysdiagramsDAL;
        public IsysdiagramsDAL sysdiagramsDAL
        {
            get
            {
                if(_sysdiagramsDAL == null)
                {
                    _sysdiagramsDAL = AbstractFactory.CreatesysdiagramsDAL();
                }
                return _sysdiagramsDAL;
            }
            set { _sysdiagramsDAL = value; }
        }
	
		private IUserInfoDAL _UserInfoDAL;
        public IUserInfoDAL UserInfoDAL
        {
            get
            {
                if(_UserInfoDAL == null)
                {
                    _UserInfoDAL = AbstractFactory.CreateUserInfoDAL();
                }
                return _UserInfoDAL;
            }
            set { _UserInfoDAL = value; }
        }
	
		private IWechatMsgInfoDAL _WechatMsgInfoDAL;
        public IWechatMsgInfoDAL WechatMsgInfoDAL
        {
            get
            {
                if(_WechatMsgInfoDAL == null)
                {
                    _WechatMsgInfoDAL = AbstractFactory.CreateWechatMsgInfoDAL();
                }
                return _WechatMsgInfoDAL;
            }
            set { _WechatMsgInfoDAL = value; }
        }
	}	
}