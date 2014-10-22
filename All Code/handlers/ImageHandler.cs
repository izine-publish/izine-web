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
	public class ImageHandler : IHttpHandler
	{
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static ImageHandler()
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
				return;

			Guid id;
			if (!iZINE.Web.Utils.Common.GuidTryParse(context.Request["id"], out id))
				return;

			int height = 0;
			int.TryParse((String)context.Request["height"], out height);

			int width = 0;
			int.TryParse((String)context.Request["width"], out width);

			String imagesDir = System.Configuration.ConfigurationManager.AppSettings["imagesDir"];

			using (iZINE.Businesslayer.iZINEEntities ctx = new iZINE.Businesslayer.iZINEEntities())
			{

				Page page = ctx.Pages.Include("Version").Where(a => a.PageId == id).FirstOrDefault();
				page.Version.AssetReference.Load();
				page.Version.Asset.TitleReference.Load();
				page.Version.Asset.ShelveReference.Load();



				imagesDir = Path.Combine(imagesDir, String.Format("{0}/{1}/{2}", page.Version.Asset.Title.TitleId, page.Version.Asset.Shelve.Date.Year.ToString(), page.Version.Asset.Shelve.ShelveId));

				String path = Path.Combine(imagesDir, String.Format("{0}.jpg", id.ToString()));
				if (!File.Exists(path))
					return;

				context.Response.Clear();
				context.Response.ContentType = "image/jpg";
				context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}.jpg\"", id.ToString()));
				context.Response.BufferOutput = false;

				context.Response.StatusCode = 200;

				try
				{
					System.Drawing.Bitmap originalBitmap = new System.Drawing.Bitmap(path);

					if (height == 0 && width == 0)
					{
						height = originalBitmap.Height;
						width = originalBitmap.Width;
					}
					else if (height == 0)
					{
						height = (int)(originalBitmap.Height * ((double)width / originalBitmap.Width));
					}
					else if (width == 0)
					{
						width = (int)(originalBitmap.Width * ((double)height / originalBitmap.Height));
					}

					if (width > originalBitmap.Width)
						width = originalBitmap.Width;

					if (height > originalBitmap.Height)
						height = originalBitmap.Height;

					System.Drawing.Bitmap thumbnailBitmap = new System.Drawing.Bitmap(width, height);

					decimal lnRatio;

					int lnNewWidth = 0;
					int lnNewHeight = 0;

					if (originalBitmap.Width > originalBitmap.Height)
					{
						lnRatio = (decimal)width / originalBitmap.Width;
						lnNewWidth = width;
						decimal lnTemp = originalBitmap.Height * lnRatio;
						lnNewHeight = (int)lnTemp;
					}
					else
					{
						lnRatio = (decimal)height / originalBitmap.Height;
						lnNewHeight = height;
						decimal lnTemp = originalBitmap.Width * lnRatio;
						lnNewWidth = (int)lnTemp;
					}

					Graphics g = Graphics.FromImage(thumbnailBitmap);
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
					g.DrawImage(originalBitmap, 0, 0, lnNewWidth, lnNewHeight);

					thumbnailBitmap.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);

					originalBitmap.Dispose();
					thumbnailBitmap.Dispose();

				}
				catch (Exception e)
				{
					throw e;
				}
			}
		}

        // used by the call to Image::GetThumbnailImage in ImageResizeHandler::ProcessRequest
        protected static bool ThumbnailImageAbortDelegate() { return false; }

	}
}
