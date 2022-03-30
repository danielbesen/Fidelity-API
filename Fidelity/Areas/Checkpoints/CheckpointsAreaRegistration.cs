using System.Web.Mvc;

namespace Fidelity.Areas.Checkpoints
{
    public class CheckpointsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Checkpoints";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Checkpoints_default",
                "Checkpoints/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}