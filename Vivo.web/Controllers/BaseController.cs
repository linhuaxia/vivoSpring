using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.IBLL;
using Vivo.Model;
using Vivo.BLLFactory;


namespace Vivo.web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        protected IPrizeResultInfoService PrizeResultBLL = AbstractFactory.CreatePrizeResultInfoService();
        protected IPlanInfoService PlanBLL = AbstractFactory.CreatePlanInfoService();
        protected IAiQiYiInfoService AiQiYiBLL = AbstractFactory.CreateAiQiYiInfoService();
        protected IProfilesInfoService ProfilesBLL = AbstractFactory.CreateProfilesInfoService();
        protected IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
    }
}