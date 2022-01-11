using System.Web.Mvc;

namespace Fidelity.Areas.Loyalt
{
    public class LoyaltAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Loyalt";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Loyalt_default",
                "Loyalt/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}