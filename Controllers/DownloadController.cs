using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Businesslayer;
using MvcPaging;
using iZINE.Web.Utils;
using System.IO;
using System.Configuration;

namespace iZINE.Web.MVC.Controllers
{
    public class DownloadController : Controller
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
        public ActionResult Index(int? page)
        {
            int pageIndex = 0;
            if(page.HasValue)
                pageIndex = page.Value;

            var downloads = Downloads.GetDownloads();

            IEnumerable<iZINE.Web.MVC.Models.DownloadModal> downloadsForBinding = (from d in downloads
                                      orderby d.CreationDate
                                      select new iZINE.Web.MVC.Models.DownloadModal
                                      {
                                          DownloadId = d.DownloadId,
                                          Description = d.Description,
                                          DownloadName = d.Name
                                      }).ToPagedList(pageIndex,5);

            return View("default", downloadsForBinding);
        }
    }
    public class AdminDownloadController : Controller
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
        public ActionResult AdminIndex(int? page)
        {
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value -1 ;

            var downloads = Downloads.GetDownloads();

            IEnumerable<iZINE.Web.MVC.Models.DownloadModal> downloadsForBinding = (from d in downloads
                                                                                   orderby d.CreationDate
                                                                                   select new iZINE.Web.MVC.Models.DownloadModal
                                                                                   {
                                                                                       DownloadId = d.DownloadId,
                                                                                       Description = d.Description,
                                                                                       DownloadName = d.Name,
                                                                                       FileName = d.Filename,
                                                                                       Date = d.CreationDate
                                                                                   }).ToPagedList(pageIndex, 5);

            return View("default", downloadsForBinding);
        }
       
        
        [Authorize]
        public ActionResult Create()
        {
            iZINE.Businesslayer.Download download = new iZINE.Businesslayer.Download();
            return View("editdownload", download);
        }

               
        [Authorize]
        public ActionResult EditDownload(FormCollection fc, HttpPostedFileBase fuDownloadFile)
        {
            try
            {
               if(fc["cancelBtn"] != null)
                   return RedirectToAction("adminindex", new { page = 1 });

                Guid downloadId;
                Utils.Common.GuidTryParse(fc["downloadId"],out downloadId);
                iZINE.Businesslayer.Download download = GetDownloadById(downloadId);
                TryUpdateModel(download, new[] { "Name", "Description" });
               
                if (fc.GetValue("Name") == null || fc["Name"] == "")
                {
                    ModelState.AddModelError("Name", "Name can not be blank");
                }

                if (!ModelState.IsValid)
                {
                    if(downloadId == Guid.Empty)
                        download = new iZINE.Businesslayer.Download();

                    return View("editdownload", download);
                }
                
               
                if (fuDownloadFile != null)
                {
                    download.Filename = GetSelectedFileName(fuDownloadFile.FileName);
                    download.MimeType = fuDownloadFile.ContentType;
                    download.Size = fuDownloadFile.ContentLength;
                
                }
                if (download.EntityState == System.Data.EntityState.Detached)
                    DataContext.AddToDownloads(download);

                DataContext.SaveChanges();

                if (fuDownloadFile != null)
                {
                    if (fuDownloadFile.ContentLength > 0)
                    {
                        string serverpath = Server.MapPath(ConfigurationManager.AppSettings["downloadsVirtualDir"]);

                        if (!Directory.Exists(serverpath))
                        {
                            Directory.CreateDirectory(serverpath);
                        }

                        fuDownloadFile.SaveAs(serverpath + "\\" + download.DownloadId.ToString());
                    }
                }

                
            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }

            return RedirectToAction("adminindex", new { page = 1 });
        }

        [Authorize]
        public ActionResult EditDownloadView()
        {
            Guid downloadId ;
            Utils.Common.GuidTryParse(Request.Params["downloadId"],out downloadId);
            iZINE.Businesslayer.Download download =  DataContext.Downloads.FirstOrDefault(d => d.DownloadId == downloadId);
            //Rewrite the internal path            
            HttpContext.RewritePath("HttpContext_Next.aspx");
            return View("editdownload", download);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();

            iZINE.Businesslayer.Download download = Downloads.GetByID(id.Value);

            DataContext.DeleteObject(download);

            DataContext.SaveChanges();
            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }

        public iZINE.Businesslayer.Download GetDownloadById(Guid? downloadId)
        {

            iZINE.Businesslayer.Download download;
            if (!downloadId.HasValue || downloadId.Value == Guid.Empty)
            {
                download = new iZINE.Businesslayer.Download();
                download.DownloadId = Guid.NewGuid();
                download.CreationDate = DateTime.Now;
                download.User = DataContext.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as iZINE.Businesslayer.User;
            }
            else
            {
                download = Downloads.GetByID(downloadId.Value);
            }
            

            return (download);
            
        }

        private string GetSelectedFileName(string fullName)
        {
            if (fullName.Contains("\\"))
            {
                return fullName.Substring(fullName.LastIndexOf("\\") + 1);
            }
            else
            {
                return fullName;
            }
        }

    }
}
