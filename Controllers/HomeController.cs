using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iZINE.Web.MVC.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var view = View("Index");
            view.MasterName = "MemberSite";
            return view;
        }

        
        public ActionResult About()
        {
            return View();
        }
    }
}
