//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/TitleController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-06-04 17:06:36 +0530 (Fri, 04 Jun 2010) $

//    $Revision: 1332 $

//    $Author: prakash.bhatt $

//    Description: controller class for title

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Web.Utils;
using iZINE.Web.MVC.Models;
using iZINE.Businesslayer;
using MvcPaging;

namespace iZINE.Web.MVC.Controllers
{
    public class TitleController : Controller
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
        public ActionResult index()
        {
             ViewData["Organisation"] = GetOrganisations(null);
             return View("default");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();

            iZINE.Businesslayer.Title title = DataContext.Titles.FirstOrDefault(t => t.TitleId == id);
            title.Active = false;
            DataContext.SaveChanges();


            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }
        

        protected IEnumerable<TitleModel> GetTitles(Guid? organizationId)
        {
            iZINE.Businesslayer.Organization organization = DataContext.Organizations.Include("Titles").
                FirstOrDefault(o => o.OrganizationId.CompareTo(organizationId.Value) == 0) as Organization;

            IEnumerable<TitleModel> titleList = from a in organization.Titles.Where(x => x.Active == true)
                                           select new TitleModel
                                           {
                                               Name = a.Name,
                                               TitleId = a.TitleId
                                           };
            return titleList;
        }

        [Authorize]
        public ActionResult GetList(Guid? Organisation,int? page)
        {
            int pageIndex = 0;
            if(page.HasValue)
                pageIndex = page.Value -1;

            ViewData["Organisation"] = GetOrganisations(Organisation);
            ViewData["orgSelected"] = Organisation;

            if (!Organisation.HasValue)
                return View("default");

            return View("default", GetTitles(Organisation).ToPagedList(pageIndex,5));
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetJsonList(Guid? Organisation, int? page)
        {
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;

            var titleList = GetTitles(Organisation);

            JsonResult result = new JsonResult();
            result.Data = new { count = titleList.Count(), items = (from i in titleList orderby i.Name descending select i).Skip(pageIndex * 5).Take(5) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;

            
        }

        private SelectList GetOrganisations(Guid? orgSelected)
        {

            var organizations = from o in DataContext.Organizations
                                where o.Active == true
                                orderby o.Name
                                select o;

            var selectList = new SelectList(organizations.ToList(), "OrganizationId", "Name", orgSelected);
            return selectList;
        }

        private IEnumerable<StatusModel> GetAllStatus()
        {
            var allStatus = from o in DataContext.Statuses
                            where o.Active == true
                            orderby o.Name
                            select new StatusModel { Name = o.Name, StatusId = o.StatusId };

            return allStatus.ToList();
        }

        public ActionResult EditTitleView(string tName, Guid? titleId)
        {
            iZINE.Businesslayer.Title title = GetTitleById(titleId);
            
            Guid? orgSelected = null;
            
            if (titleId.HasValue)
                orgSelected = title.Organization.OrganizationId;            
            
            ViewData["Organisation"] = GetOrganisations(orgSelected);
            
            ViewData["orgSelected"] = orgSelected;
            
            ViewData["titleId"] = titleId;

            ViewData["status"] = GetAllStatus();

            ViewData["statusByTitle"] = GetAllStatusForTitle(titleId);

            ViewData["usedStatus"] = getUsedStatusByTitleId(titleId);
                       
            return View("edittitle", title);
        }



        public ActionResult EditTitle(Guid? titleId, Guid? Organisation, string button)
        {
            try
            {
                if (button == null && Organisation.HasValue)
                {
                    
                    iZINE.Businesslayer.Title title = GetTitleById(titleId);
                    iZINE.Businesslayer.title_status titleStatus = new title_status();
                    iZINE.Businesslayer.Status objstatus = new Status();
                    UpdateModel(title, new[] { "Name" });

                    

                    iZINE.Businesslayer.Organization organization = DataContext.Organizations.FirstOrDefault(o => o.OrganizationId == Organisation.Value);
                    title.Organization = organization;

                    if (title.EntityState == System.Data.EntityState.Detached)
                    {
                        DataContext.AddToTitles(title);                        
                    }

                    //if a new title is inserted or an old one
                    Guid? tempTitleId = new Guid();
                    if (titleId != null)
                        tempTitleId = titleId;
                    else
                        tempTitleId = title.TitleId;

                    //To handle the status checkboxes on the form   
                   
                    #region for layout

                    foreach (string key in Request.Form.Keys)
                    {
                        if (key.StartsWith("SessionLayout:") || key.StartsWith("SessionLayout-disabled:"))
                            {
                                
                                var checkBox = Request.Form[key.ToString()];
                                int index = key.ToString().IndexOf(":");
                                int KeyLength = key.ToString().Length;

                                //if the checkbox is checked or disabled 
                                //(this logic is used because if the checkbox is checked and disabled then it gives value false)
                                if (checkBox != "false" || key.StartsWith("SessionLayout-disabled:"))
                                {
                                    Guid chkStatusID = new Guid(key.ToString().Substring(index, KeyLength - index).Replace(":", ""));
                                    title_status ts = DataContext.title_status.FirstOrDefault(o => o.titles.TitleId == tempTitleId && o.status.StatusId == chkStatusID && o.active == true);

                                    if (ts != null)
                                    {
                                        UpdateModel(ts, new[] { "layout", "active" });
                                        ts.layout = true;
                                        ts.active = true;
                                        if (ts.EntityState == System.Data.EntityState.Detached)
                                            DataContext.AddTotitle_status(ts);
                                    }
                                    //if there is no record found then insert a new row 
                                    else
                                    {

                                        title_status ts_empty = new title_status();
                                        UpdateModel(ts_empty, new[] { "titleid", "statusid", "layout", "text", "active" });

                                        ts_empty.id = Guid.NewGuid();
                                        ts_empty.titles = title;
                                        objstatus = GetStatusById(chkStatusID);
                                        ts_empty.status = objstatus;
                                        ts_empty.layout = true;
                                        ts_empty.text = false;
                                        ts_empty.active = true;

                                        if (ts_empty.EntityState == System.Data.EntityState.Detached)
                                            DataContext.AddTotitle_status(ts_empty);
                                    }
                                }
                                //if the checkbox is unchecked and find a record with titleid and statusid then set layout=false in table
                                else
                                {
                                    Guid chkStatusID = new Guid(key.ToString().Substring(index, KeyLength - index).Replace(":", ""));
                                    title_status ts = DataContext.title_status.FirstOrDefault(o => o.titles.TitleId == tempTitleId && o.status.StatusId == chkStatusID && o.active == true);
                                    if (ts != null)
                                    {

                                        UpdateModel(ts, new[] { "layout" });
                                            ts.layout = false;
                                            if (ts.EntityState == System.Data.EntityState.Detached)
                                                DataContext.AddTotitle_status(ts);
                                    }
                                }
                            }
                    }
                    #endregion
                    DataContext.SaveChanges();

                    #region for text

                    foreach (string key in Request.Form.Keys)
                    {
                        if (key.StartsWith("SessionText:") || key.StartsWith("SessionText-disabled:"))
                        {                            
                            var checkBox = Request.Form[key.ToString()];
                            int index = key.ToString().IndexOf(":");
                            int KeyLength = key.ToString().Length;

                            //if the checkbox is checked or disabled 
                            //(this logic is used because if the checkbox is checked and disabled then it gives value false)
                            if (checkBox != "false" || key.StartsWith("SessionText-disabled:"))
                            {
                                Guid chkStatusID = new Guid(key.ToString().Substring(index, KeyLength - index).Replace(":", ""));
                                title_status ts = DataContext.title_status.FirstOrDefault(o => o.titles.TitleId == tempTitleId && o.status.StatusId == chkStatusID && o.active == true);
                                if (ts != null)
                                {
                                    UpdateModel(ts, new[] { "text", "active" });
                                    ts.text = true;
                                    ts.active = true;
                                    if (ts.EntityState == System.Data.EntityState.Deleted)
                                        DataContext.AddTotitle_status(ts);
                                }
                                //if there is no record found then insert a new row 
                                else
                                {
                                    title_status ts_empty = new title_status();
                                    UpdateModel(ts_empty, new[] { "titleid", "statusid", "layout", "text", "active" });

                                    ts_empty.id = Guid.NewGuid();
                                    ts_empty.titles = title;

                                    //if the status object is not null and status id is same as the status id to be checked 
                                    //then do not assign newly created status object otherwise it will insert an extra record in title status table
                                    iZINE.Businesslayer.Status temp = GetStatusById(chkStatusID);
                                    if (objstatus != null && objstatus.StatusId == temp.StatusId)
                                    {
                                        ts_empty.status = objstatus;
                                    }
                                    else
                                    {
                                        ts_empty.status = temp;
                                    }

                                    ts_empty.layout = false;
                                    ts_empty.text = true;
                                    ts_empty.active = true;

                                    if (ts_empty.EntityState == System.Data.EntityState.Detached)
                                        DataContext.AddTotitle_status(ts_empty);
                                }
                            }
                            //if the checkbox is unchecked and find a record with titleid and statusid then set text=false in table
                            else
                            {
                                Guid chkStatusID = new Guid(key.ToString().Substring(index, KeyLength - index).Replace(":", ""));
                                title_status ts = DataContext.title_status.FirstOrDefault(o => o.titles.TitleId == titleId.Value && o.status.StatusId == chkStatusID && o.active == true);
                                if (ts != null)
                                {

                                        UpdateModel(ts, new[] { "text" });
                                        ts.text = false;
                                        if (ts.EntityState == System.Data.EntityState.Deleted)
                                            DataContext.AddTotitle_status(ts);

                                }
                            }
                        }

                    }
                    #endregion
                    DataContext.SaveChanges();

                    //if layout and text both are false and record exists in DB than deactivate the record
                    foreach (string key in Request.Form.Keys)
                    {
                        deactivateTitle_Status(key, titleId);
                    }

                                                                           
                }

            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }
            
            return RedirectToAction("getlist", new { organisation = Organisation });
        }

        /// <summary>
        /// This function is used to get a title object by titleid from 
        /// titles table and if titleid is null then a empty title with new guid.
        /// </summary>
        /// <param name="titleid"></param>
        /// <returns></returns>
        private iZINE.Businesslayer.Title GetTitleById(Guid? titleid)
        {
            
            iZINE.Businesslayer.Title title;

            if (!titleid.HasValue)
            {
                // empty title
                title = new iZINE.Businesslayer.Title();
                title.TitleId = Guid.NewGuid();
                title.Active = true;
                title.Name = "";
            }
            else
            {
                title = DataContext.Titles.FirstOrDefault(u => u.TitleId == titleid.Value);

                if (!title.OrganizationReference.IsLoaded)
                    title.OrganizationReference.Load();                      
                

                if (!title.Shelves.IsLoaded)
                    title.Shelves.Load();
                  
            }

            return (title);
           
        }


        /// <summary>
        /// This function is used to get an status object by statusid from status table
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        private iZINE.Businesslayer.Status GetStatusById(Guid? statusId)
        {
           iZINE.Businesslayer.Status status = DataContext.Statuses.FirstOrDefault(u => u.StatusId == statusId.Value && u.Active == true);
            return (status);
        }

        /// <summary>
        /// This function is used to get all the status present in title_status table
        /// </summary>
        /// <param name="titleid"></param>
        /// <returns></returns>
        public IEnumerable<iZINE.Web.MVC.Models.TitleStatusModel> GetAllStatusForTitle(Guid? titleid)
        {
           var  ts = from u in DataContext.title_status.Where(u => u.titles.TitleId == titleid.Value && u.active == true) 
                                                       select new TitleStatusModel{StatusId  = u.status.StatusId,Layout = u.layout,text=u.text};
            return (ts.ToList<iZINE.Web.MVC.Models.TitleStatusModel>());
        }

        /// <summary>
        /// This method is used to deactivate the record 
        /// from title_status table if layout and text 
        /// both are false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="titleId"></param>
        public void deactivateTitle_Status(string key, Guid? titleId)
        {
            if (key.StartsWith("SessionText:") || key.StartsWith("SessionText-disabled:"))
            {
                int index = key.ToString().IndexOf(":");
                int KeyLength = key.ToString().Length;

                Guid chkStatusID = new Guid(key.ToString().Substring(index, KeyLength - index).Replace(":", ""));
                title_status ts_deactivate = DataContext.title_status.FirstOrDefault(u => u.titles.TitleId == titleId && u.status.StatusId == chkStatusID && u.layout == false && u.text == false && u.active == true);
                if (ts_deactivate != null)
                {
                    ts_deactivate.active = false;

                    if (ts_deactivate.EntityState == System.Data.EntityState.Detached)
                        DataContext.AddTotitle_status(ts_deactivate);
                    DataContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// This function is used to find the used status in versions table and present in title_status table
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public string[] getUsedStatusByTitleId(Guid? titleId)
        {
            var statusIds = (from u in DataContext.Versions.Where(u => u.Active == true) select new { u.status.StatusId }).Distinct();
            string[] Ids = null;
            if(statusIds.Count() > 0)
            {                
                int iCount = 0;
                Ids = new string[statusIds.Count()];
                foreach (var Id in statusIds)
                {
                    Guid statusId = new Guid(Id.StatusId.ToString());
                    var isExists = from u in DataContext.title_status.Where(u => u.titles.TitleId == titleId.Value  && u.status.StatusId ==  statusId && u.active == true) select new { u.status.StatusId };
                    if (isExists.Count() > 0 && iCount < statusIds.Count())
                    {                        
                        Ids[iCount] = Id.StatusId.ToString();
                        iCount++;
                    }
                }
            }
            return Ids;
        }
    }
}
