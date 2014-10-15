//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Global.asax.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-02 16:42:40 +0530 (Fri, 02 Jul 2010) $

//    $Revision: 1614 $

//    $Author: prakash.bhatt $

//    Description: Global initialitions

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;
using System.Web.UI;
using System.IO;
using log4net.Config;

namespace iZINE.Web.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Account", action = "LogOn", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Status",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Status", action = "Status", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "StatusList",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Status", action = "GetList", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "C_PdfDelete",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "pdf", action = "DeleteLinkHandler", assetId = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "cssImages",                                              // Route name
                "cssimages/{imagename}",                           // URL with parameters
                new { controller = "cssimages", action = "getimagepath", name = "" }  // Parameter defaults
            );
			routes.MapRoute(
				"home",                                              // Route name
				"home/{index}",                           // URL with parameters
				new { controller = "home", action = "index", name = "" }  // Parameter defaults
			);


           

        }

		protected void InitializeLog4Net()
		{
			string appDir = AppDomain.CurrentDomain.BaseDirectory;
			string configFileName = appDir + "log4net.config";
			if (!File.Exists(configFileName))
			{
				throw new Exception("File : " + configFileName + " not found");
			}
			FileInfo configFile = new FileInfo(configFileName);
			XmlConfigurator.ConfigureAndWatch(configFile);
		}

        protected void Application_Start()
        {
			InitializeLog4Net();
			ControllerBuilder.Current.DefaultNamespaces.Add("iZINE.Web.MVC.Controllers");

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
            
        }

        
        
    }
}