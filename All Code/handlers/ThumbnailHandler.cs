using System;
using System.Collections.Specialized;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.UI.WebControls;
using System.IO;
using System.Globalization;
using System.Configuration;
using System.Linq;

using iZINE.Businesslayer;
using iZINE.Web.Utils;

namespace iZINE.Web.Handlers
{
	public class ThumbnailHandler : IHttpHandler
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static ThumbnailHandler()
		{
		}

		public bool IsReusable
		{
			get { return true; }
		}

        private static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            // cache forever
            // context.Response.Cache.SetCacheability(HttpCacheability.Private);
            // context.Response.Cache.SetExpires(DateTime.Now.AddYears(100));

            if (context.Request["id"] == null)
                return;

            Guid id;
            if (!iZINE.Web.Utils.Common.GuidTryParse(context.Request["id"], out id))
                return;

			using (iZINE.Businesslayer.iZINEEntities ctx = new iZINE.Businesslayer.iZINEEntities())
			{

				Page page = ctx.Pages.Include("Version").Where(a=>a.PageId== id). FirstOrDefault();
				page.Version.AssetReference.Load();
				page.Version.Asset.TitleReference.Load();
				page.Version.Asset.ShelveReference.Load();
				
				
			
				String thumbnailsDir = System.Configuration.ConfigurationManager.AppSettings["thumbnailsDir"];
				thumbnailsDir = Path.Combine(thumbnailsDir, String.Format("{0}/{1}/{2}", page.Version.Asset.Title.TitleId, page.Version.Asset.Shelve.Date.Year.ToString(), page.Version.Asset.Shelve.ShelveId));
				String path = Path.Combine(thumbnailsDir, String.Format("{0}.jpg", id.ToString()));
				if (!File.Exists(path))
					return;
	            
				context.Response.Clear();
				context.Response.ContentType = "image/jpg";
				context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}.jpg\"", id.ToString()));
				context.Response.BufferOutput = false;

				context.Response.StatusCode = 200;

				context.Response.WriteFile(path);
			}
        }
	}
}
