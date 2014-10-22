using System.Web.Mvc;

namespace iZINE.Web.MVC.Areas.AdminUser
{
    public class AdminUserAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AdminUser";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AdminUser_default",
                "AdminUser/{controller}/{action}/{id}",
                new { action = "Index", id = "" }
            );
        }
    }
}
