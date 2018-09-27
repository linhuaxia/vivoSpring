using Vivo.IBLL;
using Vivo.Model;
using Vivo.BLLFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vivo.web.Areas.MP.Controllers
{
    public class BaseController : Controller
    {

        public BaseController()
        {
        }
        public UserInfo CurrentUser;


        protected const int PageSize = 20;  //每页记录数
        protected int TheCount = 0;     //记录总数

        protected ILogInfoService LogBLL = AbstractFactory.CreateLogInfoService();
        protected IProfilesInfoService ProfilesBLL = AbstractFactory.CreateProfilesInfoService();
        protected IRuleInfoService RuleBLL = AbstractFactory.CreateRuleInfoService();
        protected IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
        protected IWechatMsgInfoService WechatMsgBLL = AbstractFactory.CreateWechatMsgInfoService();

        protected IPrizeResultInfoService PrizeResultBLL = AbstractFactory.CreatePrizeResultInfoService();
        protected IPlanInfoService PlanBLL = AbstractFactory.CreatePlanInfoService();
        protected IAiQiYiInfoService AiQiYiBLL = AbstractFactory.CreateAiQiYiInfoService();

    }
}