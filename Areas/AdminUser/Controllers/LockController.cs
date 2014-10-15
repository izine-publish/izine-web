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
	public class LockController : Controller
	{
		protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		protected static iZINE.Businesslayer.iZINEEntities DataContext
		{
			get
			{
				return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
			}
		}

		public ActionResult Index(int? page)
		{
			int pageIndex = 0;
			if (page.HasValue)
				pageIndex = page.Value - 1;
			IEnumerable<iZINE.Web.MVC.Areas.AdminUser.Models.UnLockModel> unlockModel = (from a in DataContext.Assets.Include("Locks").Include("Locks.Users").Include("Titles")
																						 where (a.Active == true)
																						 orderby a.Name
																						 select new iZINE.Web.MVC.Areas.AdminUser.Models.UnLockModel
																						 {
																							 assetId = a.AssetId,
																							 lockId = a.Lock.LockId,
																							 userId = a.Lock.Owner.UserId,
																							 applicationName = a.Lock.ApplicationName,
																							 assetName = a.Name,
																							 titleName = a.Title.Name,
																							 docName = a.Lock.DocumentName,
																							 userName = a.Lock.Owner.FirstName + " " + a.Lock.Owner.LastName,
																							 date = a.Date.Value

																						 }).ToPagedList(pageIndex, 15);



			return View("default", unlockModel);
		}


		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult DeleteLinkHandler(Guid? id)
		{
			var result = new JsonResult();
			iZINEEntities context = DataContextFactory.GetDataContext<iZINEEntities>();

			Asset asset = context.Assets.FirstOrDefault<Asset>(a => a.AssetId == id && a.Active == true);

			asset.Lock = null;

			context.SaveChanges();
			result.Data = new { Success = "success" };
			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;

		}
	}
}
