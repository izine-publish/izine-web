//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/PdfController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-02 11:47:36 +0530 (Fri, 02 Jul 2010) $

//    $Revision: 1601 $

//    $Author: prakash.bhatt $

//    Description: controller class for certified Pdf

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Services;
using MvcPaging;
using System.Linq.Expressions;
using System.IO;

using iZINE.Businesslayer;
using iZINE.Web.Utils;
using iZine.Helpers;
using iZINE.Web.MVC.Models;

namespace iZINE.Web.MVC.Controllers
{
    public class PdfController : Controller
    {
        protected static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }

        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);
        }
        //
        // GET: /Status/
        [Authorize]
        public ActionResult Index()
        {
			if (Session["MyChoice"] != null)
			{
				MyChoice myChoice = (MyChoice)Session["MyChoice"];
				return RedirectToAction("getList", new { plank = myChoice.editionId, titel = myChoice.titleId, page = 1 });
			}

            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
            var values = from c in user.Titles
                         where c is Title 
                         orderby c.Name
                         select c;

            var selectList = new SelectList(values.ToList(), "titleid", "Name");
            ViewData["titel"] = selectList;

			ViewData["plank"] = new SelectList(new System.Collections.ArrayList());
            return View("default");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetShelves(Guid? id)
        {
            var result = new JsonResult();

            if (id.HasValue)
            {
                var planks = from s in DataContext.Shelves
                             where (s.Title.TitleId == id && s.Active == true)
                             orderby s.Name
                             select new { id = s.ShelveId, name = s.Name };
                result.Data = new { values = planks };
            }
            else
            {
                var planks = new SelectList(new System.Collections.ArrayList());
                result.Data = new { values = planks };
            }


            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plank"></param>
        /// <param name="titel"></param>
        /// <param name="page"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public ActionResult GetList(Guid? plank, Guid? titel, int? page,Guid? docId)
        {
            //iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;

            var values = from c in DataContext.Titles
                         where c is Title && c.Active == true
                         orderby c.Name
                         select c;

            var selectList = new SelectList(values.ToList(), "titleid", "Name", titel);
            ViewData["titel"] = selectList;
            ViewData["tSelected"] = titel;
            var planks = from s in DataContext.Shelves
                         where s.Title.TitleId == titel && s.Active == true 
                         orderby s.Name
                         select new { id = s.ShelveId, name = s.Name };

            var PlankList = new SelectList(planks.ToList(), "id", "Name", plank);
            ViewData["plank"] = PlankList;
            ViewData["pSelected"] = plank;

            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;


            Guid gPlank = Guid.Empty;
            if (plank.HasValue)
                gPlank = plank.Value;

            return View("default", GetAssets(gPlank).ToPagedList(pageIndex, 10));
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetJsonList(Guid? plank, Guid? titel, int? page)
        {
            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;

            
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;


            Guid gPlank = Guid.Empty;
            if (plank.HasValue)
                gPlank = plank.Value;

            var docList = GetAssets(gPlank);

			//add to session variable 
			Session.Add("MyChoice", new MyChoice { titleId = titel.Value, editionId = plank.Value });

            JsonResult result = new JsonResult();
            result.Data = new { count = docList.Count(), items = (from i in docList orderby i.DocumentId descending select i).Skip(pageIndex * 10).Take(10) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        private IEnumerable<iZINE.Web.MVC.Models.C_PdfModel> GetAssets(Guid shelveId)
        {
            if (shelveId == Guid.Empty)
            {
                return (null);
            }

            IEnumerable<iZINE.Web.MVC.Models.C_PdfModel> assetList = (from a in DataContext.Assets.Include("Version").Include("Version.User")
                            where a.Shelve.ShelveId == shelveId && a.Active == true && a.Type.ConstantId == Constant.PDF_C
                            orderby a.Version.Date descending
                            select new iZINE.Web.MVC.Models.C_PdfModel {Name = a.Name,
                                VersionId= a.Version.VersionId,
                                Date= a.Date,VersionNumber=a.Version.Number,
                                DocumentId=a.AssetId,
                                UserName=a.Version.User.FirstName +" " +a.Version.User.LastName,
                                sheleveName=a.Shelve.Name,
                                titleName=a.Shelve.Title.Name,
                                titileId= a.Shelve.Title.TitleId,
                                StatusId= a.Shelve.ShelveId
                            });

            return assetList;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();

            iZINE.Businesslayer.Asset asset = DataContext.Assets.Include("Version").FirstOrDefault(x => x.AssetId == id);

            asset.Active = false;

            DataContext.SaveChanges();

            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }

        public ActionResult EditDocument(string Name, string tName, HttpPostedFileBase uploadFile,
            Guid? docId, Guid? sheleveId, Guid? titleId, string button)
        {

            try
            {
                if (button == null)
                {
                    iZINE.Businesslayer.Type type = DataContext.Constants.FirstOrDefault(
                        t => t.ConstantId == Constant.PDF_C) as iZINE.Businesslayer.Type;

                    if (type == null)
                    {
                        throw new Exception("type not found");
                    }

                    iZINE.Businesslayer.Asset asset = GetAssetbyId(docId);


                    UpdateModel(asset, new[] { "Name" });
                    asset.Date = DateTime.Now;
                    asset.Title = GetTitlebyId(titleId);
                    asset.Shelve = GetShelvebyId(sheleveId);
                    asset.Type = type;
                    asset.Active = true;

                    int nextVersionNumber = GetAssetNextVersionNumber(asset.AssetId);

                    if (uploadFile != null)
                    {
                        iZINE.Businesslayer.Version version = new iZINE.Businesslayer.Version();

                        version.VersionId = Guid.NewGuid();
                        version.Asset = asset;
                        version.Date = DateTime.Now;
                        version.Number = nextVersionNumber;

                        iZINE.Businesslayer.User user = DataContext.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as iZINE.Businesslayer.User;
                        version.User = user;

                        version.Active = true;
                        version.Filename = GetSelectedFileName(uploadFile.FileName);
                        asset.Filename = GetSelectedFileName(uploadFile.FileName);

                        ValidateFileType(uploadFile.FileName);
                       asset.ShelveReference.Load();
					   asset.TitleReference.Load();

						String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];
						assetsDir = Path.Combine(assetsDir, String.Format("{0}/{1}/{2}", asset.Title.TitleId, asset.Shelve.Date.Year.ToString(), asset.Shelve.ShelveId));
						if (uploadFile.ContentLength > 0)
                        {
                            if (!Directory.Exists(assetsDir))
                                Directory.CreateDirectory(assetsDir);
                            uploadFile.SaveAs(Path.Combine(assetsDir, String.Format("{0}.pdf", version.VersionId.ToString())));
                        }



                        asset.Version = version;
                    }
                    else
                    {
                        if (nextVersionNumber == 1)
                        {
                            // it's the first version and we need a file to be uploaded
                            throw new Exception("File is empty");
                        }
                    }



                    // 
                    // save all changes in a transaction
                    //
                    DataContext.SaveChanges();


                }
            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }

        
            return RedirectToAction("getlist",new{plank = GetShelvebyId(sheleveId).ShelveId, titel = GetTitlebyId(titleId).TitleId, page =1,docid =GetAssetbyId(docId).AssetId});
        }
        [Authorize]
        public ActionResult Create(Guid? sheleveId, Guid? titleId)
        {
            Asset asset = new Asset();
            if (sheleveId.HasValue)
                ViewData["shName"] = GetShelvebyId(sheleveId).Name;
            else
                ViewData["shName"] = "";

            if (titleId.HasValue)
                ViewData["tName"] = GetTitlebyId(titleId).Name;
            else
                ViewData["tName"] = "";



            ViewData["sheleveId"] = sheleveId;
            ViewData["titleId"] = titleId;

            return View("Edit", asset);

        }

        [Authorize]
        public ActionResult EditDocumentView(Guid? docId)
        {
            Asset asset = GetAssetbyId(docId);

            ViewData["shName"] = asset.Shelve.Name;
            
            ViewData["tName"] = asset.Title.Name;
            
            ViewData["sheleveId"] = asset.Shelve.ShelveId;
            ViewData["titleId"] = asset.Title.TitleId;
            ViewData["docId"] = docId;

            return View("Edit", asset);
        }

        private int GetAssetNextVersionNumber(Guid assetId)
        {
            int nextVersionNumber = 1;

            var versionNumbers = from v in DataContext.Versions
                                 where v.Asset.AssetId == assetId
                                 select v.Number;

            if (versionNumbers.Count() > 0)
            {
                nextVersionNumber = versionNumbers.Max().Value + 1;
            }

            return nextVersionNumber;
        }

        private string GetSelectedFileName(string fileName)
        {
            string fullName = fileName;

            if (fullName.Contains("\\"))
            {
                return fullName.Substring(fullName.LastIndexOf("\\") + 1);
            }
            else
            {
                return fullName;
            }
        }

        private void ValidateFileType(string fileName)
        {
            fileName = fileName.ToLower();

            if (!fileName.EndsWith(".pdf"))
            {
                throw new Exception("Invalid file type. PDF expected");
            }
        }

        private iZINE.Businesslayer.Asset GetAssetbyId(Guid? assetId)
        {
            
                iZINE.Businesslayer.Asset asset;

                if (!assetId.HasValue)
                {
                    asset = new iZINE.Businesslayer.Asset();
                    asset.AssetId = Guid.NewGuid();
                    asset.Active = true;

                }
                else
                {
                    Guid assetid = assetId.Value;
                    asset = DataContext.Assets.Include("Shelve").Include("Title").FirstOrDefault(a => a.AssetId == assetid);
                    
                }

                return asset;
        }

        public iZINE.Businesslayer.Shelve GetShelvebyId(Guid? sheleveId)
        {
            iZINE.Businesslayer.Shelve shelve;

            if (!sheleveId.HasValue)
            {
                return (null);
            }
            else
            {
                Guid shelveid = sheleveId.Value;
                shelve = DataContext.Shelves.FirstOrDefault(u => u.ShelveId == shelveid);
            }

            return shelve;
        }

        public iZINE.Businesslayer.Title GetTitlebyId(Guid? titleId)
        {
            
            iZINE.Businesslayer.Title title;

            if (!titleId.HasValue)
            {
                return (null);
            }
            else
            {
                Guid titleid = titleId.Value;
                title = DataContext.Titles.FirstOrDefault(u => u.TitleId == titleid);
            }

            return title;
        }
            

    }
}
