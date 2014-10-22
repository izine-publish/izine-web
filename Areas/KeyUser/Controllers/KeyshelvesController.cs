using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcPaging;
using iZINE.Web.Utils;
using iZINE.Businesslayer;
using iZINE.Web.MVC.Models;


namespace iZINE.Web.MVC.Areas.KeyUser.Controllers
{
	public class KeyshelvesController : Controller
	{
		protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{

			base.OnActionExecuting(filterContext);
		}
		protected static iZINE.Businesslayer.iZINEEntities DataContext
		{
			get
			{
				return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
			}
		}
		[Authorize]
		public ActionResult Index()
		{
			iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(ControllerContext.HttpContext.User.Identity.Name) == 0) as User;
			var organizations = from o in DataContext.Organizations
								join u in DataContext.Users on o.OrganizationId equals u.Organization.OrganizationId
								where (o.Active == true && u.UserId == user.UserId)
								orderby o.Name
								select o;

			//var selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name");
			List<SelectListItem> selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name").ToList();
			selectList.Insert(0, new SelectListItem { Text = "[selecteer een organisatie]", Value = "", Selected = true });
			ViewData["Organisation"] = selectList;

			ViewData["titel"] = new SelectList(new System.Collections.ArrayList());
			return View("Keydefault");

		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult GetShelves(Guid? id)
		{
			var result = new JsonResult();

			if (id.HasValue)
			{


				var shelveList = from c in DataContext.Titles
								 where c
								 is Title && c.Organization.OrganizationId == id && c.Active == true
								 orderby c.Name
								 select new { titleId = c.TitleId, name = c.Name };

				result.Data = new { values = shelveList };


			}
			else
			{
				var shelveList = new SelectList(new System.Collections.ArrayList());
				result.Data = new { values = shelveList };
			}


			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;


		}

		[OutputCache(Duration = 0, VaryByParam = "None")]
		public JsonResult GetShelveList(Guid? Organisation, Guid? titel, int? page)
		{
			int pageIndex = 0;
			if (page.HasValue)
				pageIndex = page.Value - 1;

			var shelveList = (from a in DataContext.Shelves
							  where a.Title.TitleId == titel && a.Active == true
							  orderby a.Name descending
							  select new { Name = a.Name, SheleveId = a.ShelveId });
			JsonResult result = new JsonResult();
			result.Data = new { count = shelveList.Count(), items = (from i in shelveList orderby i.Name descending select new { id = i.SheleveId, name = i.Name }).Skip(pageIndex * 5).Take(5) };
			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;

		}

		public ActionResult GetList(Guid? Organisation, Guid? titel, int? page)
		{
			//organisation list
			iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(ControllerContext.HttpContext.User.Identity.Name) == 0) as User;
			var organizations = from o in DataContext.Organizations
								join u in DataContext.Users on o.OrganizationId equals u.Organization.OrganizationId
								where (o.Active == true && u.UserId == user.UserId)
								orderby o.Name
								select o;

			//var selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name", Organisation);
			List<SelectListItem> selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name").ToList();
			selectList.Insert(0, new SelectListItem { Text = "[selecteer een organisatie]", Value = "", Selected = true });
			ViewData["Organisation"] = selectList;
			ViewData["OrgSelected"] = Organisation;

			//title list
			var tileList = from c in DataContext.Titles
						   where c
							   is Title && c.Organization.OrganizationId == Organisation
						   orderby c.Name
						   select new { titleId = c.TitleId, name = c.Name };

			var titleSelectList = new SelectList(tileList.ToList(), "titleId", "Name", titel);
			ViewData["titel"] = titleSelectList;
			ViewData["tSelected"] = titel;


			int pageIndex = 0;
			if (page.HasValue)
				pageIndex = page.Value - 1;

			IEnumerable<iZINE.Web.MVC.Areas.KeyUser.Models.SheleveModel> shelveList = (from a in DataContext.Shelves
																					   where a.Title.TitleId == titel && a.Active == true
																					   orderby a.Name descending
																					   select new iZINE.Web.MVC.Areas.KeyUser.Models.SheleveModel { Name = a.Name, SheleveId = a.ShelveId }).ToPagedList(pageIndex, 5);
			return View("Keydefault", shelveList);
		}

		public ActionResult Create()
		{
			iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(ControllerContext.HttpContext.User.Identity.Name) == 0) as User;
			var organizations = from o in DataContext.Organizations
								join u in DataContext.Users on o.OrganizationId equals u.Organization.OrganizationId
								where (o.Active == true && u.UserId == user.UserId)
								orderby o.Name
								select o;

			Shelve shelve = new Shelve();

			//var selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name");
			List<SelectListItem> selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name").ToList();
			selectList.Insert(0, new SelectListItem { Text = "[selecteer een organisatie]", Value = "", Selected = true });

			ViewData["Organisation"] = selectList;

			var tileList = from c in DataContext.Titles
						   where c
							   is Title && c.Active == true
						   orderby c.Name
						   select new { titleId = c.TitleId, name = c.Name };

			var titleSelectList = new SelectList(tileList.ToList(), "titleId", "Name");
			ViewData["titel"] = titleSelectList;


			return View("KeyEditShelve", shelve);
		}

		public ActionResult EditShelveView(Guid? sheleveId)
		{
			iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(ControllerContext.HttpContext.User.Identity.Name) == 0) as User;
			var organizations = from o in DataContext.Organizations
								join u in DataContext.Users on o.OrganizationId equals u.Organization.OrganizationId
								where (o.Active == true && u.UserId == user.UserId)
								orderby o.Name
								select o;

			Shelve shelve = GetShelveById(sheleveId);

			var selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name", shelve.Title.Organization.OrganizationId);
			ViewData["Organisation"] = selectList;

			var tileList = from c in DataContext.Titles
						   where c
							   is Title && c.Organization.OrganizationId == shelve.Title.Organization.OrganizationId
						   orderby c.Name
						   select new { titleId = c.TitleId, name = c.Name };

			var titleSelectList = new SelectList(tileList.ToList(), "titleId", "Name", shelve.Title.TitleId);
			ViewData["titel"] = titleSelectList;


			return View("KeyEditShelve", shelve);
		}

		public ActionResult EditShelve(Guid? sheleveId, string button)
		{
			iZINE.Businesslayer.Shelve shelve = GetShelveById(sheleveId);
			Guid titelid = Guid.Empty;
			try
			{
				if (button == null)
				{

					TryUpdateModel(shelve, new[] { "Name" });


					iZINE.Web.Utils.Common.GuidTryParse(Request.Form["titel"].ToString(), out titelid);



					iZINE.Businesslayer.Title title = DataContext.Titles.FirstOrDefault(t => t.TitleId == titelid);
					shelve.Title = title;

					if (shelve.EntityState == System.Data.EntityState.Detached)
						DataContext.AddToShelves(shelve);

					DataContext.SaveChanges();
				}
			}
			catch (Exception exc)
			{
				log.Error(exc);
				throw;
			}
			return RedirectToAction("getlist");
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult DeleteLinkHandler(Guid? id)
		{
			var result = new JsonResult();

			iZINE.Businesslayer.Shelve shelve = DataContext.Shelves.FirstOrDefault(t => t.ShelveId == id);
			shelve.Active = false;
			DataContext.SaveChanges();

			result.Data = new { Success = "success" };
			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;


		}


		private iZINE.Businesslayer.Shelve GetShelveById(Guid? sheleveId)
		{

			iZINE.Businesslayer.Shelve shelve;

			if (!sheleveId.HasValue || sheleveId.Value == Guid.Empty)
			{
				// empty title
				shelve = new iZINE.Businesslayer.Shelve();
				shelve.ShelveId = Guid.NewGuid();
				shelve.Active = true;
			}
			else
			{
				shelve = DataContext.Shelves.FirstOrDefault(u => u.ShelveId == sheleveId);

				if (!shelve.TitleReference.IsLoaded)
					shelve.TitleReference.Load();

				if (!shelve.Title.OrganizationReference.IsLoaded)
					shelve.Title.OrganizationReference.Load();
			}

			return (shelve);

		}


	}
}
