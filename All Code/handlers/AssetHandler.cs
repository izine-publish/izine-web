//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/All Code/handlers/AssetHandler.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-02 11:47:36 +0530 (Fri, 02 Jul 2010) $

//    $Revision: 1601 $

//    $Author: prakash.bhatt $

//    Description: handler class for asset

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

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
	public class AssetHandler : IHttpHandler
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static AssetHandler()
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

			Guid versionid;
			if (!iZINE.Web.Utils.Common.GuidTryParse(context.Request["id"], out versionid))
				return;

			iZINE.Businesslayer.Version version = DataContext.Versions.Include("Asset").Include("Asset.Type").FirstOrDefault<iZINE.Businesslayer.Version>(a => a.VersionId == versionid && a.Active == true);
			if (version == null)
				return;

			iZINE.Businesslayer.Asset asset = version.Asset;

			asset.TypeReference.Load();
			asset.ShelveReference.Load();
			asset.TitleReference.Load();

			String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];
			assetsDir = Path.Combine(assetsDir, String.Format("{0}/{1}/{2}", asset.Title.TitleId, asset.Shelve.Date.Year.ToString(),asset.Shelve.ShelveId));

			String fileName = asset.Name;

			String path = null;

			String contentType = "application/octet-stream";

			if (asset.Type.ConstantId == new Guid("a3871d22-7d8b-4d9f-901e-4c284b1a801e"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.indd", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("BDEC0BF8-9591-4533-AFEA-467D0B340E92"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.incx", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("756779a5-bb56-4430-9e95-022226970095"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.incx", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("0DB0B0BC-25A8-4B5E-81B1-819336832D68"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.inct", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("2326e21b-f4fb-40eb-acf6-46454d4d9f9f"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.indt", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("b4eee6f9-6061-481e-8a6e-f89dfbebad4e"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.inca", asset.Version.VersionId.ToString()));
			}
			else if (asset.Type.ConstantId == new Guid("88cfa137-d9c9-415c-9745-67921bb77f47"))
			{
				path = Path.Combine(assetsDir, String.Format("{0}.pdf", asset.Version.VersionId.ToString()));
				contentType = "application/pdf";
				fileName = String.Format("{0}.pdf", asset.Name);
			}
			else
			{
				throw new Exception("Unknown type");
			}

			if (!File.Exists(path))
				throw new Exception("Could not find file."); ;

			System.IO.Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

			context.Response.Clear();
			context.Response.ContentType = contentType;

			context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", fileName));
			context.Response.BufferOutput = false;

			context.Response.StatusCode = 200;


			byte[] buffer = new byte[4096];
			int count;

			while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				context.Response.OutputStream.Write(buffer, 0, count);
			}

			context.Response.Flush();
			inputStream.Close();

		}
	}
}
