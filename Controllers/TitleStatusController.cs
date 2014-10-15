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

namespace iZINE.Web.MVC.Controllers
{
    public class TitleStatusController : Controller
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
        // GET: /TitleStatus/

        [Authorize]
        public ActionResult Index(int? page)
        {
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;

            IEnumerable<TitleStatusModel> statuses = (from s in DataContext.Statuses
                                                      where s.Active == true
                                                      orderby s.Name
                                                      select new TitleStatusModel
                                                      {
                                                          Name = s.Name,
                                                          StatusId = s.StatusId,
                                                      }).ToPagedList(pageIndex, 15);

            return View("default", statuses);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult JsonIndex(int? page)
        {
            JsonResult result = new JsonResult();
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;
            IEnumerable<TitleStatusModel> statuses = (from o in DataContext.Statuses
                                                      where o.Active == true
                                                      orderby o.Name
                                                      select new TitleStatusModel { Name = o.Name, StatusId = o.StatusId });



            result.Data = new { count = statuses.Count(), items = (from i in statuses orderby i.StatusId descending select i).Skip(pageIndex * 15).Take(15) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
        
        [Authorize]
        public ActionResult EditStatusView(Guid? statusId, int? page)
        {
            iZINE.Businesslayer.Status titlestatus = GetStatusById(statusId);
            ViewData["page"] = page;
            ViewData["statusId"] = statusId;

            return View("edittitlestatus", titlestatus);
        }

        [Authorize]
        public ActionResult EditStatus(FormCollection fc, int? page, Guid? statusId, string button)
        {
            try
            {
                if (button == null)
                {
                    iZINE.Businesslayer.Status titlestatus = GetStatusById(statusId);

                    TryUpdateModel(titlestatus, new[] { "Name" });

                    // valdiate uniqueness.
                    IEnumerable<TitleStatusModel> status = from p in DataContext.Statuses
                                                           where p.Name.ToUpper() == titlestatus.Name.ToUpper()
                                                           select new TitleStatusModel
                                                           {
                                                               Name = p.Name,
                                                               StatusId = p.StatusId
                                                           };

                    if (fc["Name"] == null || fc["Name"] == "")
                    {
                        ModelState.AddModelError("Name", "Required");
                    }
                    else if (status.Count() > 0)
                    {
                        ModelState.AddModelError("Name1", "NotUniuqe");
                    }
                    if (!ModelState.IsValid)
                        return View("edittitlestatus", titlestatus);

                    if (titlestatus.EntityState == System.Data.EntityState.Detached)
                        DataContext.AddToStatuses(titlestatus);

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

        public iZINE.Businesslayer.Status GetStatusById(Guid? statusid)
        {
            iZINE.Businesslayer.Status status;
            if (!statusid.HasValue)
            {
                //empty status
                status = new iZINE.Businesslayer.Status();
                status.StatusId = new Guid();
                status.Active = true;
                status.Name = "";
            }
            else
            {
                status = DataContext.Statuses.FirstOrDefault(u => u.StatusId == statusid && u.Active == true);
            }
            return (status);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();
            iZINE.Businesslayer.Status status = DataContext.Statuses.FirstOrDefault(o => o.StatusId == id);
            status.Active = false;
            DataContext.SaveChanges();

            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

    }
}
