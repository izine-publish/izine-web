//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/All Code/handlers/CssImageHandler.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: handler for css image

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
namespace iZINE.Web.Handlers
{
    public class CssImageHandler : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.PhysicalPath.Contains("App_Theme"))
            {
                //write your handler implementation here.
                context.Response.Clear();
                context.Response.ContentType = getContentType(context.Request.PhysicalPath);

                String sImagePath = context.Request.PhysicalPath;
                
                if (context.Request.UserLanguages.Length > 0)
                {
                    foreach(string lang in context.Request.UserLanguages)
                    {
                        sImagePath = sImagePath.Replace("images","images\\"+lang);
                        if(File.Exists(sImagePath))
                            break;
                    }
                    
                }

				if (File.Exists(sImagePath))
					context.Response.WriteFile(sImagePath);
                context.Response.End();
            }
        }

        string getContentType(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
                default: break;
            }
            return "";
        }
        
        ImageFormat getImageFormat(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: break;
            }
            return ImageFormat.Jpeg;
        }

        #endregion
    }
}
