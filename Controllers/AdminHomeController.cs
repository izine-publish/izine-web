using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace iZINE.Web.MVC.Controllers
{
    public class AdminHomeController : Controller
    {
        [Authorize]
        public ActionResult IndexAdmin()
        {
            var view = View("index");
            view.MasterName = "AdminSite";
            return view;
        }


    }
}
