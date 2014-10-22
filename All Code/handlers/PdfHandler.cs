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
    public class PdfHandler : IHttpHandler
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static PdfHandler()
        {
        }

        private static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // cache forever
            // context.Response.Cache.SetCacheability(HttpCacheability.Private);
            // context.Response.Cache.SetExpires(DateTime.Now.AddYears(100));
            if (context.Request["id"] == null)
                throw new Exception("no id");

            Guid versionid;
            if (!iZINE.Web.Utils.Common.GuidTryParse(context.Request["id"], out versionid))
                throw new Exception("not a valid id");

            iZINE.Businesslayer.Version version = DataContext.Versions.FirstOrDefault<iZINE.Businesslayer.Version>(v => v.VersionId == versionid && v.Active == true);
            if (version == null)
                throw new Exception("version not found");

            version.AssetReference.Load();
			version.Asset.TitleReference.Load();
			version.Asset.ShelveReference.Load();

            
            String pdfDir = System.Configuration.ConfigurationManager.AppSettings["pdfDir"];

			pdfDir = Path.Combine(pdfDir, String.Format("{0}/{1}/{2}", version.Asset.Title.TitleId, version.Asset.Shelve.Date.Year.ToString(), version.Asset.Shelve.ShelveId.ToString()));

            String path = Path.Combine(pdfDir, String.Format("{0}.pdf", version.VersionId.ToString()));
            if (!File.Exists(path))
                throw new Exception("could not find path " + path);

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}.pdf\"", version.Asset.Name));
            context.Response.BufferOutput = false;

            context.Response.StatusCode = 200;

            context.Response.WriteFile(path);
            context.Response.End();
            
        }
    }
}
