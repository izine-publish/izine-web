using System.Web.Mvc;

namespace iZINE.Web.MVC.Areas.KeyUser
{
    public class KeyUserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "KeyUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "KeyUser_default",
                "KeyUser/{controller}/{action}/{id}",
                new { action = "Index", id = "" }
            );
        }
    }
}
