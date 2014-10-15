using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Web.MVC.Models;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using MvcPaging;

namespace iZINE.Web.MVC.Areas.KeyUser.Controllers
{
    public class KeyOrganisationController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }
        //
        // GET: /Organisation/

        [Authorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;
            iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(ControllerContext.HttpContext.User.Identity.Name) == 0) as User;
            IEnumerable<iZINE.Web.MVC.Areas.KeyUser.Models.OrganisationModel> organizations = (from o in DataContext.Organizations 
                                                            join u in DataContext.Users 
                                                            on o.OrganizationId equals u.Organization.OrganizationId
                                                            where (o.Active == true && u.Role.RoleId == new Guid("1313712B-7675-450A-A0D3-C774366DFE45") && u.UserId == user.UserId)
                                                            orderby o.Name
                                                            select new iZINE.Web.MVC.Areas.KeyUser.Models.OrganisationModel { Name = o.Name, OrganisationId = o.OrganizationId }).ToPagedList(pageIndex, 15);



            return View("Keydefault", organizations);
        }
       
    }
}
