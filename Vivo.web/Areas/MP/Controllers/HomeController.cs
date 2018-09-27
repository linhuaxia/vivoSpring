using Vivo.BLLFactory;
using Vivo.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;
using System.Data.Entity;

namespace Vivo.web.Areas.MP.Controllers
{

    [AllowAnonymous]
    public class HomeController : BaseMPUnFilterController
    {
        // GET: MP/Home
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult menu()
        //{
        //    var menus = PowerActionBLL.GetByUserID(CurrentUser.ID).Where(p => p.Enable && p.TypeEnum == (int)SysEnum.PowerActionType.菜单权限).ToList();


        //    return View(menus);
        //}
        [AllowAnonymous]
        public ActionResult menuTemp()
        {

            return View();
        }
        [AllowAnonymous]
        public ActionResult Note()
        {
           
            return View();
        }
        public ActionResult Top()
        {
            IUserInfoService UserBLL = AbstractFactory.CreateUserInfoService();
            return View(UserBLL.GetCurrent());
        }

    }
}