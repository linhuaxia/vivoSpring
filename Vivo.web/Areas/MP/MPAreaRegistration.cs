using System.Web.Mvc;

namespace Vivo.web.Areas.MP
{
    public class MPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MP_default",
                "MP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Vivo.web.Areas.MP.Controllers" }
            );
        }
    }
}