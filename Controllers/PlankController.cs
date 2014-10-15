//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/PlankController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: controller class for plank

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Text;
using System.Web.Services;
using MvcPaging;
using System.Linq.Expressions;
using System.IO;

using iZine.Helpers;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using iZINE.Web.MVC.Models;

namespace iZINE.Web.MVC.Controllers
{
    public class PlankController : Controller
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
            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
            IEnumerable<SelectListItem> values = (from c in user.Titles
                                                  where c is Title
                                                  orderby c.Name
                                                  select new SelectListItem { Value = c.TitleId.ToString(), Text = c.Name });
			if (Session["MyChoice"] != null)
			{
				MyChoice myChoice = (MyChoice)Session["MyChoice"];
				return RedirectToAction("pubIndex", new { title = myChoice.titleId, plank = myChoice.editionId });
			}

            ViewData["plank"] = new SelectList(new System.Collections.ArrayList());
			
            return View("default", values);
        }

        [Authorize]
        public ActionResult PubIndex(Guid? title,Guid? plank)
        {
            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
            IEnumerable<SelectListItem> values = (from c in user.Titles
                                                  where c is Title
                                                  orderby c.Name
                                                  select new SelectListItem { Value = c.TitleId.ToString(), Text = c.Name ,Selected= c.TitleId== title.Value ? true : false});
            

           var planks = from s in DataContext.Shelves
                         where s.Title.TitleId == title && s.Active == true
                         orderby s.Name
                         select new { id = s.ShelveId, name = s.Name };

            ViewData["plank"] = new SelectList(planks.ToList(), "id", "name", plank);


            return View("default", values);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Plank(Guid? id)
        {
            var result = new JsonResult();

            if (id.HasValue)
            {
   
                var planks = from s in DataContext.Shelves
                             where s.Title.TitleId == id && s.Active==true
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

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetJsonList(Guid? plank,Guid? titel)
        {
            JsonResult result = new JsonResult();
            List<DocumentGroupPage> plankList = new List<DocumentGroupPage>();
            do
            {
                if (!plank.HasValue)
                    break;

                iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
           
            
                plankList = GetPages(plank.Value);
            
            
                
               
            } while (false);

			//add to session variable 
			Session.Add("MyChoice", new MyChoice { titleId = titel.Value, editionId = plank.Value });

            result.Data = new { count = plankList.Count(), items = (from i in plankList select i) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

            
        }
        private List<DocumentGroupPage> GetPages(Guid shelveId)
        {
            // don't know why I have to sort twice
            Guid typeId = new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e");
            var pages = (from p in
                             (
                                 from d
                                     in DataContext.Assets.Include("status").Include("Status.StatusState")
                                 from p in d.Version.Pages
                                 orderby p.Number
                                 where d.Type.ConstantId == typeId && d.Shelve.ShelveId == shelveId && d.Active == true
                                 select new DocumentPagePreviewData
                                 {
                                     Number = p.Number,
                                     Type = p.Type,
                                     PageId = p.PageId,
                                     AssetId = d.AssetId,
                                     VersionId = d.Version.VersionId,
                                     Name = d.Name,
                                     StatusId = d.Version.status.StatusId,
									 StatusState = d.Version.status.StatusStates.StateId
                                 }
                                 )
                         orderby p.Number
                         select p);

            List<DocumentPagePreviewData> allPages = new List<DocumentPagePreviewData>();

            allPages.AddRange(pages.ToList());


            List<DocumentPagePreviewData> extendedPageList = new List<DocumentPagePreviewData>();

            Guid? previousPageType = null;

            int i = 0;

            while (i < allPages.Count)
            {
                var currentPage = allPages[i];

                if (currentPage.Type.ConstantId == Constant.LeftPageType)
                {
                    if (previousPageType.HasValue && previousPageType.Value == Constant.LeftPageType)
                    {
                        // this is a left page and the previous one was left so we need to add a spare on as separator
                        extendedPageList.Add(new DocumentPagePreviewData());
                    }
                    else
                    {
                        // this is the first page, or the previous page was either right or unisex which is ok
                    }
                }
                else if (currentPage.Type.ConstantId == Constant.RightPageType)
                {
                    if (previousPageType.HasValue && previousPageType.Value == Constant.RightPageType)
                    {
                        // we have a right page before that so we need a separator
                        extendedPageList.Add(new DocumentPagePreviewData());
                    }
                    else
                    {
                        if (!previousPageType.HasValue)
                        {
                            //it's right and it's the first so we need to shift right
                            extendedPageList.Add(new DocumentPagePreviewData());
                        }
                        // it was either left or uni-sex so we are ok
                    }
                }
                else
                {
                    // the page was uni-sex so we don't care what was its predcessor 
                }

                extendedPageList.Add(currentPage);
                previousPageType = currentPage.Type.ConstantId;
                i++;
            }

            if (pages.Count() > 0)
            {
                var firstpage = allPages.First();

                if (firstpage.Type.ConstantId != Constant.LeftPageType || firstpage.Type.ConstantId != Constant.UnisexPageType)
                {
                    DocumentPagePreviewData zeroPage = new DocumentPagePreviewData();
                    allPages.Add(zeroPage);
                }
            }



            if (extendedPageList.Count % 2 != 0)
            {
                if (extendedPageList.Count() > 0)
                {
                    DocumentPagePreviewData endPage = new DocumentPagePreviewData();
                    extendedPageList.Add(endPage);
                }
            }

            foreach (DocumentPagePreviewData page in extendedPageList)
            {
                if (page.StatusId == Guid.Empty)
                    page.IsBlank = true;
            }
            List<DocumentGroupPage> dataSource = new List<DocumentGroupPage>();

            for (int j = 0; j < extendedPageList.Count; j = j + 2)
            {
                DocumentGroupPage group = new DocumentGroupPage();
                group.First = extendedPageList[j];
                group.Second = extendedPageList[j + 1];
				group.First.StatusImage = GetImageUrl(group.First.StatusId);
				group.Second.StatusImage = GetImageUrl(group.Second.StatusId);
                dataSource.Add(group);
            }

            return dataSource;

            
        }

		private string GetImageUrl(Guid StatusId)
		{
			StringBuilder imageUrlPath = new StringBuilder("/App_Themes/green/images/");

			Status status = DataContext.Statuses.Include("StatusStates").FirstOrDefault(n => n.StatusId == StatusId);
			if(status == null)
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
