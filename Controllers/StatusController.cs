//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/StatusController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-08-10 21:49:42 +0530 (Tue, 10 Aug 2010) $

//    $Revision: 1941 $

//    $Author: remco.verhoef $

//    Description: controller class for status

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using System.Web.Services;
using System.Text;

using iZine.Helpers;
using MvcPaging;
using System.Linq.Expressions;
using iZINE.Web.MVC.Extensions;
using iZINE.Web.MVC.Models;

namespace iZINE.Web.MVC.Controllers
{
	public class StatusController : Controller
	{
		private static iZINE.Businesslayer.iZINEEntities DataContext
		{
			get
			{
				return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
			}
		}
		public StatusController()
		{
			this.View().MasterName = "MemberSite";

		}

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Guid assetid, FormCollection fc)
        {
            iZINE.Web.MVC.Models.Status.Edit model = new iZINE.Web.MVC.Models.Status.Edit();

            using (iZINEEntities ctx = new iZINEEntities())
            {
                var asset = ctx.Assets.Include("Shelve").Include("Shelve.Title").ById(assetid);
                model.Asset = asset;

                var titles = from c in ctx.Titles
                             where c is Title && c.Active == true
                             orderby c.Name
                             select c;

                model.Titles = new SelectList(titles.ToList(), "TitleId", "Name", asset.Title.TitleId);

                var shelves = (from c in ctx.Shelves.Include("Title")
                               where c is Shelve && c.Title.TitleId == asset.Title.TitleId
                               orderby c.Name
                               select c).ToList();

                model.Shelves = new SelectList(shelves.ToList(), "ShelveId", "Name", asset.Shelve.ShelveId);

                var status = (from c in ctx.Statuses
                              orderby c.Name
                              select c).ToList();
                model.Status = new SelectList(status.ToList(), "StatusId", "Name", null);

                asset.Name=fc["name"];
                ctx.SaveChanges();
            }

            if (Request.IsAjaxRequest() || true)
            {
                JsonResult result = new JsonResult();
                result.Data = true;
                return (result);
            }
            else
            {
                return (View(model));
            }
        }

        [Authorize]
        public ActionResult Edit(Guid assetid)
        {
            iZINE.Web.MVC.Models.Status.Edit model = new iZINE.Web.MVC.Models.Status.Edit();

            using (iZINEEntities ctx = new iZINEEntities())
            {
                var asset = ctx.Assets.Include("Shelve").Include("Shelve.Title").ById(assetid);
                model.Asset = asset;

                var titles = from c in ctx.Titles
                             where c is Title && c.Active == true
                             orderby c.Name
                             select c;

                model.Titles = new SelectList(titles.ToList(), "TitleId", "Name", asset.Title.TitleId);
            
                var shelves = (from c in ctx.Shelves.Include("Title")
                               where c is Shelve && c.Title.TitleId == asset.Title.TitleId
                               orderby c.Name
                               select c).ToList();

                model.Shelves = new SelectList(shelves.ToList(), "ShelveId", "Name", asset.Shelve.ShelveId);

                var status = (from c in ctx.Statuses
                              orderby c.Name
                              select c).ToList();
                model.Status = new SelectList(status.ToList(), "StatusId", "Name", null);
            }

            return (View(model));
        }

		//
		// GET: /Status/
		[Authorize]
		public ActionResult Index()
        {
            Guid? groupid = null;

            if (Request.Cookies["groupid"] != null && !String.IsNullOrEmpty(Request.Cookies["groupid"].Value))
                groupid = new Guid(Request.Cookies["groupid"].Value);
            
            Guid? titleid=null;

            if (Request.Cookies["titleid"] != null && !String.IsNullOrEmpty(Request.Cookies["shelveid"].Value))
                titleid=new Guid(Request.Cookies["titleid"].Value);

            Guid? shelveid = null;

            if (Request.Cookies["shelveid"]!=null && !String.IsNullOrEmpty(Request.Cookies["shelveid"].Value))
                shelveid = new Guid(Request.Cookies["shelveid"].Value);

            iZINE.Web.MVC.Models.Status.Index model = new iZINE.Web.MVC.Models.Status.Index();

            using (iZINEEntities ctx = new iZINEEntities())
            {
                var groups = from c in ctx.Titles
                             where c is Title && c.Active == true
                             orderby c.Name
                             select c;

                if (groupid.HasValue)
                {
                    model.Groups = new SelectList(groups.ToList(), "TitleId", "Name", "Alle");
                }
                else
                {
                    model.Groups = new SelectList(groups.ToList(), "TitleId", "Name");
                }

                var titles = from c in ctx.Titles
                             where c is Title && c.Active==true
                             orderby c.Name
                             select c;

                if (titleid.HasValue)
                {
                    model.Titles = new SelectList(titles.ToList(), "TitleId", "Name", titleid);
                }
                else
                {
                    model.Titles = new SelectList(titles.ToList(), "TitleId", "Name");
                }
                 
                if (titleid.HasValue)
                {
                    var shelves = (from c in ctx.Shelves.Include("Title")
                                  where c is Shelve && c.Title.TitleId == titleid.Value
                                  orderby c.Name
                                  select c).ToList();

                    model.Shelves = new SelectList(shelves.ToList(), "ShelveId", "Name", shelveid);
                }
                else
                {
                    model.Shelves = new SelectList(new SelectListItem[]{});
                }

                if (shelveid.HasValue)
                {
                    var assets = from c in ctx.Assets.Include("Shelve")
                                 where c is Asset && c.Shelve.ShelveId == shelveid.Value && c.Active==true
                                 orderby c.Name
                                 select c;

                    model.Assets = assets.ToList();

                    

			Guid typeId = new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e");

			IList<iZINE.Web.MVC.Models.StatusModel> assetList = (from d in
																		   (from a in ctx.Assets.Include("Version").Include("Lock").Include("Lock.Owner").Include("Lock.Owner.Membership").Include("Version.Pages").Include("Version.Status").Include("Users")
																			from p in a.Version.Pages
																			where a.Shelve.ShelveId == shelveid && a.Active == true && a.Type.ConstantId == typeId
																			select new
																			{
																				DocumentId = a.AssetId,
																				StatusId = a.Version.status.StatusId,
																				a.Name,
																				a.Date,
																				VersionId = a.Version.VersionId,
																				VersionNumber = a.Version.Number,
																				PageNumber = p.Number,
																				userFirstName = a.Version.User.FirstName,
																				userMiddleName = a.Version.User.MiddleName,
																				userLastName = a.Version.User.LastName,
																				isLocked = a.Lock != null ? true:false,
																				lockedBy = a.Lock != null ? a.Lock.Owner.FirstName + " " + a.Lock.Owner.MiddleName + " " + a.Lock.Owner.LastName : null
																			}
																		   )
																	   group d by new
																	   {
																		   d.DocumentId,
																		   d.Name,
																		   d.StatusId,
																		   d.Date,
																		   d.VersionId,
																		   d.VersionNumber,
																		   d.PageNumber,
																		   d.userFirstName,
																		   d.userMiddleName,
																		   d.userLastName,
																		   d.isLocked,
																		   d.lockedBy
																	   } into dg
																	   orderby dg.Key.PageNumber
																	   select new iZINE.Web.MVC.Models.StatusModel
																	   {

																		   DocumentId = dg.Key.DocumentId,
																		   Name = dg.Key.Name,
																		   StatusId = dg.Key.StatusId,
																		   VersionId = dg.Key.VersionId,
																		   Date = dg.Key.Date,
																		   VersionNumber = dg.Key.VersionNumber,
																		   PageNumber = dg.Key.PageNumber,
																		   UserFName = dg.Key.userFirstName,
																		   UserMName = dg.Key.userMiddleName,
																		   UserLName = dg.Key.userLastName,
																		   IsLocked = dg.Key.isLocked,
																		   LockedBy = dg.Key.lockedBy

																	   }).ToList();

			//assetList = assetList.UniqueBy(u => u.PageNumber);
			foreach (iZINE.Web.MVC.Models.StatusModel stMo in assetList)
			{
				stMo.ImageUrl = GetImageUrl(stMo.StatusId);

				Guid DocumentId = stMo.DocumentId;

				Guid atypeId = new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e");
				List<iZINE.Web.MVC.Models.statusAssignment> assignmentsList = (from a in DataContext.Assets.Include("Document").Include("Version.Status").Include("Users")

																			   where a.Type.ConstantId == atypeId && a.Document.AssetId == DocumentId && a.Active == true
																			   orderby a.Name
																			   select new iZINE.Web.MVC.Models.statusAssignment
																			   {
																				   statusid = a.Version.status.StatusId,
																				   Name = a.Name,
																				   VersionNumber = a.Version.Number,
																				   Date = a.Version.Date,
																				   UserFName = a.Version.User.FirstName,
																				   UserMName = a.Version.User.MiddleName,
																				   UserLName = a.Version.User.LastName,
																				   
																			   }).ToList();


				foreach (var asMo in assignmentsList)
				{
					if (asMo.statusid.HasValue)
						asMo.url = GetImageUrl(asMo.statusid.Value);
					else
						asMo.url = stMo.ImageUrl;
				}

				stMo.AssignmentsList = assignmentsList;
			}
            model.Assetlist = assetList;
                }
                else {
                    model.Assets = new Asset[] { };
                    model.Assetlist = new StatusModel[] { };
                }
            }

            if (Request.IsAjaxRequest())
            {
                return (PartialView("Content", model));
            }
            else
            {
                return View(model);
            }
        }

		public ActionResult Shelve(Guid id)
		{
			var result = new JsonResult();

            using (iZINEEntities ctx = new iZINEEntities())
            {
                var shelves = (from s in ctx.Shelves.Include("Title")
                              where s.Active == true && s.Title.TitleId ==id
                              orderby s.Name
                              select new { id = s.ShelveId, name = s.Name }).ToList();
                result.Data = shelves;

            }

			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;


		}


		[OutputCache(Duration = 0, VaryByParam = "None")]
		public JsonResult GetJsonList(Guid? plank, Guid? titel)
		{
			iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;


			Guid typeId = new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e");

			IEnumerable<iZINE.Web.MVC.Models.StatusModel> assetList = (from d in
																		   (from a in DataContext.Assets.Include("Version").Include("Lock").Include("Lock.Owner").Include("Lock.Owner.Membership").Include("Version.Pages").Include("Version.Status").Include("Users")
																			from p in a.Version.Pages
																			where a.Shelve.ShelveId == plank && a.Active == true && a.Type.ConstantId == typeId
																			select new
																			{
																				DocumentId = a.AssetId,
																				StatusId = a.Version.status.StatusId,
																				a.Name,
																				a.Date,
																				VersionId = a.Version.VersionId,
																				VersionNumber = a.Version.Number,
																				PageNumber = p.Number,
																				userFirstName = a.Version.User.FirstName,
																				userMiddleName = a.Version.User.MiddleName,
																				userLastName = a.Version.User.LastName,
																				isLocked = a.Lock != null ? true:false,
																				lockedBy = a.Lock != null ? a.Lock.Owner.FirstName + " " + a.Lock.Owner.MiddleName + " " + a.Lock.Owner.LastName : null
																			}
																		   )
																	   group d by new
																	   {
																		   d.DocumentId,
																		   d.Name,
																		   d.StatusId,
																		   d.Date,
																		   d.VersionId,
																		   d.VersionNumber,
																		   d.PageNumber,
																		   d.userFirstName,
																		   d.userMiddleName,
																		   d.userLastName,
																		   d.isLocked,
																		   d.lockedBy
																	   } into dg
																	   orderby dg.Key.PageNumber
																	   select new iZINE.Web.MVC.Models.StatusModel
																	   {

																		   DocumentId = dg.Key.DocumentId,
																		   Name = dg.Key.Name,
																		   StatusId = dg.Key.StatusId,
																		   VersionId = dg.Key.VersionId,
																		   Date = dg.Key.Date,
																		   VersionNumber = dg.Key.VersionNumber,
																		   PageNumber = dg.Key.PageNumber,
																		   UserFName = dg.Key.userFirstName,
																		   UserMName = dg.Key.userMiddleName,
																		   UserLName = dg.Key.userLastName,
																		   IsLocked = dg.Key.isLocked,
																		   LockedBy = dg.Key.lockedBy

																	   }).ToList();

			//assetList = assetList.UniqueBy(u => u.PageNumber);
			foreach (iZINE.Web.MVC.Models.StatusModel stMo in assetList)
			{
				stMo.ImageUrl = GetImageUrl(stMo.StatusId);

				Guid DocumentId = stMo.DocumentId;

				Guid atypeId = new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e");
				List<iZINE.Web.MVC.Models.statusAssignment> assignmentsList = (from a in DataContext.Assets.Include("Document").Include("Version.Status").Include("Users")

																			   where a.Type.ConstantId == atypeId && a.Document.AssetId == DocumentId && a.Active == true
																			   orderby a.Name
																			   select new iZINE.Web.MVC.Models.statusAssignment
																			   {
																				   statusid = a.Version.status.StatusId,
																				   Name = a.Name,
																				   VersionNumber = a.Version.Number,
																				   Date = a.Version.Date,
																				   UserFName = a.Version.User.FirstName,
																				   UserMName = a.Version.User.MiddleName,
																				   UserLName = a.Version.User.LastName,
																				   
																			   }).ToList(); ;


				foreach (var asMo in assignmentsList)
				{
					if (asMo.statusid.HasValue)
						asMo.url = GetImageUrl(asMo.statusid.Value);
					else
						asMo.url = stMo.ImageUrl;
				}

				stMo.AssignmentsList = assignmentsList;
			}

			//add to session variable 
			Session.Add("MyChoice", new MyChoice { titleId = titel.Value, editionId = plank.Value });

			JsonResult result = new JsonResult();
			result.Data = new { count = assetList.Count(), items = assetList };
			result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			return result;
		}


		private string GetImageUrl(Guid StatusId)
		{
			StringBuilder imageUrlPath = new StringBuilder("/App_Themes/green/images/");
			
			Status status = DataContext.Statuses.Include("StatusStates").FirstOrDefault(n=> n.StatusId == StatusId);
			if (status == null)
				imageUrlPath.Append("status1");

			else if (new Guid("AB136AE9-728A-421D-873B-EAE32D5E33C7").CompareTo(status.StatusStates.StateId) == 0)
			{
				// eindredactie
				imageUrlPath.Append("status3");

			}
			else if (new Guid("DA43364A-0F49-4735-B88B-674B6D20639B").CompareTo(status.StatusStates.StateId) == 0)
			{
				// vormgeving
				imageUrlPath.Append("status1");

			}
			else if (new Guid("E0501419-3C1B-4833-9FCE-EB8138F9C304").CompareTo(status.StatusStates.StateId) == 0)
			{
				// redactie
				imageUrlPath.Append("status2");

			}
			else if (new Guid("A40327CF-5190-4694-94ED-BABAEDA98C3F").CompareTo(status.StatusStates.StateId) == 0)
			{
				// definitief
				imageUrlPath.Append("status4");

			}
			return imageUrlPath.Append("18x18.png").ToString();
		}




	}
}
