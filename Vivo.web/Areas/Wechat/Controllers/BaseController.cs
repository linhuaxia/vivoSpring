using Eval.IBLL;
using Eval.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eval.web.Areas.Wechat.Controllers
{
    public class BaseController : Controller
    {

        public UserInfo CurrentUser;


        protected const int PageSize = 20;  //每页记录数
        protected int TheCount = 0;     //记录总数
    }
}