//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Extensions/HtmlHelper.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: extention methods for html helper class

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Linq.Expressions;
using System.Web.Mvc.Html;

namespace iZINE.Web.MVC.Extensions
{
    public static class ImageHelper
    {

        public static MvcHtmlString Image(this HtmlHelper helper, string StatusId) 
        {
            string imageUrlPath = "";
            if ("0b75fc2d-eda3-4a56-a6ea-27dc9970e80b".CompareTo(StatusId) == 0)
            {
                // eindredactie
                imageUrlPath = "../../images/status3.gif";
               
            }
            else if ("a350e418-e5b0-4fe5-92ee-66d9814e29c2".CompareTo(StatusId) == 0)
            {
                // vormgeving
                imageUrlPath = "../../images/status1.gif";
                
            }
            else if ("69c76023-3cf2-4a58-90d7-ad410435f47f".CompareTo(StatusId) == 0)
            {
                // redactie
                imageUrlPath = "../../images/status2.gif";
               
            }
            else if ("22f8acbd-8e7c-4437-a767-e5773db76083".CompareTo(StatusId) == 0)
            {
                // definitief
                imageUrlPath = "../../images/status4.gif";
               
            }
            //<img id="sImage" src="../../images/status2.gif" alt="test"/>
            string outputUrl = string.Format(@"<img src='{0}' style='border-style: none' id='sImage' alt='test'/>", imageUrlPath);
            return MvcHtmlString.Create(outputUrl.ToString());
        }

        public static string DeleteLink(this HtmlHelper html
              , string linkText
              , string routeName
              , object routeValues)
                    {
                        var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
                        string url = urlHelper.RouteUrl(routeName, routeValues);

                        string format = @"<form method=""post"" action=""{0}"" 
              class=""delete-link"">
            <input type=""submit"" value=""{1}"" />
            {2}
            </form>";

                        string form = string.Format(format, html.AttributeEncode(url)
                          , html.AttributeEncode(linkText)
                          , html.AntiForgeryToken());
                        return form + html.RouteLink(linkText, routeName, routeValues
                        , new { @class = "delete-link", style = "display:none;" });
        }

        public static string MenuActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName,string className,string activeClass)
        {
            
            string sHtml = "<div><a href='/";
            sHtml += controllerName;
            sHtml +="/" +actionName +"'" + "class='";

            if (helper.ViewContext.Controller.GetType().Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase))
            {
                sHtml += activeClass;
            }
            else
            {
                sHtml += className ;
            }
            sHtml += "'></a></div>";
            return sHtml;
        }
        public static string MenuActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, string className, string activeClass, string areaName)
        {
            string sHtml = "<div><a href='/";
            sHtml += areaName;
            sHtml += "/" + controllerName;
            sHtml += "/" + actionName + "'" + "class='";

            if (helper.ViewContext.Controller.GetType().Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase))
            {
                sHtml += activeClass;
            }
            else
            {
                sHtml += className;
            }
            sHtml += "'></a></div>";
            return sHtml;
        }

        public static  MvcHtmlString TopMenuActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName,object routeValues)
        {
            
            if (helper.ViewContext.Controller.GetType().Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase))
            {
                if(routeValues != null)
                    return helper.ActionLink(linkText, actionName, controllerName,routeValues, new { style = "font-weight: bold;" });
                else
                    return helper.ActionLink(linkText, actionName, controllerName, null, new { style = "font-weight: bold;" });
            }
            else if ((helper.ViewContext.Controller.GetType().Name.ToLower().Contains("archief")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("status")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("pdf")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("plank")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Equals("downloadcontroller"))
                && controllerName == "home")
            {
                string shtml = "<a href='/" + controllerName + "/" + actionName + "' style='font-weight: bold;'>" + linkText + "</a>";
                return MvcHtmlString.Create(shtml);
               
            }

            else if ((helper.ViewContext.Controller.GetType().Name.ToLower().Contains("organisation")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("users")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("title")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("shelves")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("admindownload"))
                && controllerName == "adminhome")
            {
                string shtml = "<a href='/" + controllerName + "/" + actionName + "' style='font-weight: bold;'>" + linkText + "</a>";
                return MvcHtmlString.Create(shtml);

                
            }
            else if (controllerName.ToLower() == "keyhome")
            {
               
                if ((helper.ViewContext.Controller.GetType().Name.ToLower().Contains("keyorganisation")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("keytitle")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("keyusers")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("Keyshelves")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("KeyTitleStatus")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("KeyDownload")))
                {

                    string shtml = "<a href='/KeyUser/" + controllerName + "/" + actionName + "' style='font-weight: bold;'>" + linkText + "</a>";
                    return MvcHtmlString.Create(shtml);
                }
                return helper.ActionLink(linkText, actionName, controllerName, routeValues, null);


            }
            else if (controllerName.ToLower() == "home")
            {

                if ((helper.ViewContext.Controller.GetType().Name.ToLower().Contains("organisation")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("usertitle")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("users")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("shelves")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("TitleStatus")
                || helper.ViewContext.Controller.GetType().Name.ToLower().Contains("Download")))
                {

                    string shtml = "<a href='/AdminUser/" + controllerName + "/" + actionName + "' style='font-weight: bold;'>" + linkText + "</a>";
                    return MvcHtmlString.Create(shtml);
                }
                return helper.ActionLink(linkText, actionName, controllerName, routeValues, null);


            }

            return helper.ActionLink(linkText, actionName, controllerName, new { area = ""}, null);
            
           
        }

       

    }
}
