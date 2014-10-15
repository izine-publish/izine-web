using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using MvcPaging;

using iZINE.Web.MVC.Areas.AdminUser.Models;

namespace iZINE.Web.MVC.Areas.AdminUser.Controllers
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

			IEnumerable<iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel> statuses = (from s in DataContext.Statuses
																						   where s.Active == true
																						   orderby s.Name
																						   select new iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel
																						   {
																							   Name = s.Name,
																							   StatusId = s.StatusId,
																							   State = s.StatusStates.Name
																							   
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
			IEnumerable<iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel> statuses = (from o in DataContext.Statuses
													  where o.Active == true
													  orderby o.Name
													  select new iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel { Name = o.Name, StatusId = o.StatusId, State = o.StatusStates.Name });



			result.Data = new { count = statuses.Count(), items = (from i in statuses orderby i.StatusId descending select i).Skip(pageIndex * 15).Take(15) };
			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;
		}

		[Authorize]
		public ActionResult EditStatusView(Guid? statusId, int? page)
		{
			iZINE.Businesslayer.Status titlestatus = GetStatusById(statusId);

			EditTitleStatusModel model = new EditTitleStatusModel();
			model.Name = titlestatus.Name;
			model.StateId = titlestatus.StatusStates == null ? (Guid?)null : titlestatus.StatusStates.StateId;
			model.StatusId = titlestatus.StatusId;
			model.States = (from n in DataContext.StatusStates select n).ToList();
			ViewData["page"] = page;
			
			return View("edittitlestatus", model);
		}

		[Authorize]
		public ActionResult EditStatus(EditTitleStatusModel model,Guid? statusId, int? page, string button)
		{
			try
			{
				if (button == null)
				{
					if (ModelState.IsValid)
					{
						iZINE.Businesslayer.Status titlestatus = GetStatusById(statusId);

						// valdiate uniqueness.
						bool check = (from t in DataContext.Statuses
									  where String.Compare(t.Name, model.Name,
									  StringComparison.InvariantCultureIgnoreCase)
									  == 0
									  select t).Any();

						if (check && (titlestatus.EntityState == System.Data.EntityState.Detached))
						{
							ModelState.AddModelError("Name1", "NotUniuqe");
							model.States = (from n in DataContext.StatusStates select n).ToList();
							return View("edittitlestatus", model);
						}

						titlestatus.Name = model.Name;
						titlestatus.StatusStates = DataContext.StatusStates.FirstOrDefault(v => v.StateId == model.StateId);

						if (titlestatus.EntityState == System.Data.EntityState.Detached)
							DataContext.AddToStatuses(titlestatus);

						DataContext.SaveChanges();

					}
					else
					{
						model.States = (from n in DataContext.StatusStates select n).ToList();
						return View("edittitlestatus", model);
					}
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
			if (statusid == Guid.Empty || !statusid.HasValue)
			{
				//empty status
				status = new iZINE.Businesslayer.Status();
				status.StatusId = Guid.NewGuid();
				status.Active = true;
				status.Name = "";
			}
			else
			{
				status = DataContext.Statuses.Include("StatusStates").FirstOrDefault(u => u.StatusId == statusid && u.Active == true);
				if (status == null)
				{
					status = new iZINE.Businesslayer.Status();
					status.StatusId = Guid.NewGuid();
					status.Active = true;
				}
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
