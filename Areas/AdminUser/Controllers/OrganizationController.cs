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

namespace iZINE.Web.MVC.Areas.AdminUser.Controllers
{
    public class OrganizationController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }
        [Authorize]
        public ActionResult index(int? page)
        {
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;
            IEnumerable<iZINE.Web.MVC.Areas.AdminUser.Models.OrganisationModel> organizations = (from o in DataContext.Organizations
                                                                                                 where o.Active == true
                                                                                                 orderby o.Name
                                                                                                 select new iZINE.Web.MVC.Areas.AdminUser.Models.OrganisationModel { Name = o.Name, OrganisationId = o.OrganizationId }).ToPagedList(pageIndex, 15);



            return View("default", organizations);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult JsonIndex(int? page)
        {
            JsonResult result = new JsonResult();
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;
            IEnumerable<iZINE.Web.MVC.Areas.AdminUser.Models.OrganisationModel> organizations = (from o in DataContext.Organizations
                                                                                                    where o.Active == true
                                                                                                    orderby o.Name
                                                                                                    select new iZINE.Web.MVC.Areas.AdminUser.Models.OrganisationModel { Name = o.Name, OrganisationId = o.OrganizationId });



            result.Data = new { count = organizations.Count(), items = (from i in organizations orderby i.OrganisationId descending select i).Skip(pageIndex * 15).Take(15) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [Authorize]
        public ActionResult EditOrganisationView(Guid? OrgId, int? page)
        {
            iZINE.Businesslayer.Organization organization = GetOrganisationById(OrgId);
            ViewData["page"] = page;
            ViewData["OrgId"] = OrgId;

            return View("EditOrganization", organization);
        }

        [Authorize]
        public ActionResult EditOrganisation(FormCollection fc, int? page, Guid? OrgId, string button)
        {
            try
            {
                if (button == null)
                {
                    iZINE.Businesslayer.Organization organization = GetOrganisationById(OrgId);
                    TryUpdateModel(organization, new[] { "Name" });
                    if (fc["Name"] == null || fc["Name"] == "")
                    {
                        ModelState.AddModelError("Name", "Required");
                    }
                    if (!ModelState.IsValid)
                        return View("EditOrganization", organization);

                    if (organization.EntityState == System.Data.EntityState.Detached)
                        DataContext.AddToOrganizations(organization);

                    DataContext.SaveChanges();


                }
            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }


            return RedirectToAction("index", new { page = page });


        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();
            iZINE.Businesslayer.Organization organization = DataContext.Organizations.FirstOrDefault(o => o.OrganizationId == id);
            organization.Active = false;
            DataContext.SaveChanges();

            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }

        private iZINE.Businesslayer.Organization GetOrganisationById(Guid? OrgId)
        {
            iZINE.Businesslayer.Organization organization;

            if (!OrgId.HasValue)
            {
                organization = new iZINE.Businesslayer.Organization();
                organization.OrganizationId = Guid.NewGuid();
                organization.Active = true;
            }
            else
            {
                organization = DataContext.Organizations.FirstOrDefault(u => u.OrganizationId == OrgId);
            }

            return organization;

        }

    }
}
