//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/ArchiefController.cs $

//    Owner: Prakash Bhatt

//    $Date: 2010-01-28 17:16:16 +0530 (Thu, 28 Jan 2010) $

//    $Revision: 770 $

//    $Author: prakash.bhatt $

//    Description: Controller class for Archief

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Xml;
using System.Xml.XPath;
using System.Configuration;
using MvcPaging;
using iZINE.Web.MVC.Models;

namespace iZINE.Web.MVC.Controllers
{
    public class ArchiefController : Controller
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);
        }

        [Authorize]
        public ActionResult Index()
        {
            return View("default");
        }

        [Authorize]
        public ActionResult GetResults(string query, string author,string title,bool? dateChecked,string fromDate,string tillDate,int? page)
        {
            String queryToSearch = "";
            try
            {
                if (query != null && query.Length == 0)
                {                    
                    return View("default");
                }

               
                if (query.Length > 0)
                {
                    queryToSearch += query;
                }

                if (author != null && author.Length > 0)
                {
                    queryToSearch += String.Format(" author: {0}", author);
                }

                /*if (!String.IsNullOrEmpty(title))
                {
                    queryToSearch += String.Format(" title: {0}", title);
                }
                */
                if (dateChecked.HasValue && dateChecked.Value == true)
                {
                    DateTime dtfromDate = DateTime.ParseExact(fromDate, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                   
                    DateTime dttillDate = DateTime.ParseExact(tillDate, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    queryToSearch += String.Format(" publicationDate:[{0} TO {1}]", dtfromDate.ToString("yyyy-MM-ddT00:00:00Z"), dttillDate.ToString("yyyy-MM-ddT00:00:00Z"));
                }

                System.Diagnostics.Debug.WriteLine(query);

               
            }
            catch (Exception exc)
            {
                log.Error(exc);

                throw;
            }
            ViewData["query"] = query;
            ViewData["author"] = author;
            ViewData["dateChecked"] = dateChecked;
            ViewData["fromDate"] = fromDate;
            ViewData["tillDate"] = tillDate;
            
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;
            return View("default", FillResults(queryToSearch).ToPagedList(pageIndex,5));
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetJsonResults(string query, string author, string title, bool? dateChecked, string fromDate, string tillDate, int? page)
        {
            JsonResult result = new JsonResult();
            String queryToSearch = "";
            try
            {
                if (query != null && query.Length == 0)
                {
                    return result;
                }


                if (query.Length > 0)
                {
                    queryToSearch += query;
                }

                if (author != null && author.Length > 0)
                {
                    queryToSearch += String.Format(" author: {0}", author);
                }

                /*if (!String.IsNullOrEmpty(title))
                {
                    queryToSearch += String.Format(" title: {0}", title);
                }
                */
                if (dateChecked.HasValue && dateChecked.Value == true)
                {
                    DateTime dtfromDate = DateTime.ParseExact(fromDate, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                    DateTime dttillDate = DateTime.ParseExact(tillDate, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    queryToSearch += String.Format(" publicationDate:[{0} TO {1}]", dtfromDate.ToString("yyyy-MM-ddT00:00:00Z"), dttillDate.ToString("yyyy-MM-ddT00:00:00Z"));
                }

                System.Diagnostics.Debug.WriteLine(query);


            }
            catch (Exception exc)
            {
                log.Error(exc);

                throw;
            }
            
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;

            List<Result> resultList = FillResults(queryToSearch);

            
            result.Data = new { count = resultList.Count, items = (from i in resultList orderby i.Id descending select i).Skip(pageIndex * 5).Take(5) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

            
        }


        protected List<Result> FillResults(string query)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            string url = String.Format(ConfigurationManager.AppSettings["SolrUrl"], query);
            log.Debug(url);

            XmlReader reader = XmlTextReader.Create(url, settings);

            XPathDocument xdoc = new XPathDocument(reader);

            XPathNavigator navigator = xdoc.CreateNavigator();
            int numFound = int.Parse(navigator.SelectSingleNode("//response/result").GetAttribute("numFound", ""));

            System.Collections.Generic.List<Result> results = new System.Collections.Generic.List<Result>();
            
            XPathNodeIterator iterator = navigator.Select("//response/result/doc");
            while (iterator.MoveNext())
            {
                System.Diagnostics.Debug.WriteLine(iterator.Current.InnerXml);

                Result result = new Result();
                result.Author = iterator.Current.SelectSingleNode("str[@name='author']/text()").Value;
                result.Id = iterator.Current.SelectSingleNode("str[@name='id']/text()").Value;
                result.Subject = iterator.Current.SelectSingleNode("str[@name='subject']/text()").Value;
                result.Title = iterator.Current.SelectSingleNode("str[@name='title']/text()").Value;
                result.PublicationDate = DateTime.Parse(iterator.Current.SelectSingleNode("date[@name='publicationDate']/text()").Value);
                result.PublicationDateString = result.PublicationDate.ToString("d-M-yyyy");
                if (iterator.Current.SelectSingleNode("str[@name='pdfFile']/text()") != null)
                {
                    result.PdfFile = iterator.Current.SelectSingleNode("str[@name='pdfFile']/text()").Value;
                }
                else
                {
                    result.PdfFile = null;
                }

                results.Add(result);
            }

            return results;
        }

    }
    
}
