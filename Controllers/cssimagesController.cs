using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;


namespace iZINE.Web.MVC.Controllers
{
    public class cssimagesController : Controller
    {
        //
        // GET: /cssimages/

        public ActionResult Index()
        {
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        public ActionResult getimagepath(string name)
        {
            
            string ss = this.ControllerContext.RequestContext.HttpContext.Request.UserLanguages.FirstOrDefault();
            return (new RedirectResult("/App_Themes/green/images/buttons/vernieuwen.png"));
            
        }

    }
}
