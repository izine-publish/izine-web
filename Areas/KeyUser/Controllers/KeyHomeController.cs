using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace iZINE.Web.MVC.Areas.KeyUser.Controllers
{
    public class KeyHomeController : Controller
    {
        //
        // GET: /KeyHome/

        public ActionResult KeyIndex()
        {
            var view = View("KeyIndex");
            view.MasterName = "KeyUser";
            return view;
        }

    }
}
