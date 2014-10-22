//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/SettingController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-06-04 17:06:36 +0530 (Fri, 04 Jun 2010) $

//    $Revision: 1332 $

//    $Author: prakash.bhatt $

//    Description: contrpller class for settings

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Collections;
using iZINE.Web.Utils;
using iZINE.Businesslayer;
using System.Web.Services;
using iZINE.Web.MVC.Models;

namespace iZINE.Web.MVC.Controllers
{
    public class SettingController : Controller
    {
        private static iZINE.Businesslayer.iZINEEntities DataContext
        {
            get
            {
                return (DataContextFactory.GetWebRequestScopedDataContext<iZINE.Businesslayer.iZINEEntities>());
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewData["titel"] = GetTitles(null);

            var statusList = from c in DataContext.Constants.ToList()
                             //where c is Status
                             orderby c.Name
                             select c;


            ViewData["status"] = statusList.ToList();

            return View("default", GetNotifications());
        }

        [Authorize]
        private List<Title> GetTitles(Guid? title)
        {
            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
            var values = from c in user.Titles
                         where c is Title
                         orderby c.Name
                         select c;

            return values.ToList();
            
        }

        private IEnumerable<NotificationModel> GetNotifications()
        {
            iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;
            IEnumerable<NotificationModel>  notifications = (from n in user.Notifications
                                                    select new NotificationModel { StatusId = n.StatusId, StatusName = n.status.Name, TitleId = n.TitleId, TitleName = n.Title.Name }).ToList();
            
            return notifications;
        }
        [Authorize]
        public ActionResult SaveNotification()
        {
            for (int i = 0; i < Request.Form.Count; i++)
            {
                var checkbox = Request.Form[i];
                var key = Request.Form.GetKey(i);
                string[] arrayString = key.Split(new[] { '+' });
                if (arrayString.Length != 2)
                    continue;
                Guid statusId = new Guid(arrayString[0]);
                Guid titleId = new Guid(arrayString[1]);

                 // the reason you check for false because the Html checkbox helper does some kind of freaky thing for value true: it makes the string read "true, false"
                if (checkbox != "false")
                {
                    AddNotification(statusId, titleId);
                }
                else
                {
                    DeleteNotification(statusId, titleId);
                }
            }

            DataContext.SaveChanges();
           
            return RedirectToAction("index");
        }

        private void AddNotification(Guid statusId, Guid titleId)
        {
            //do
            //{
            //    iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;


            //    Title title = DataContext.Titles.FirstOrDefault(t => t.TitleId == titleId);

            //    Status status = DataContext.Constants.FirstOrDefault(c => c.ConstantId == statusId) as Status;

            //    user.Notifications.Load();

            //    UserNotification un = user.Notifications.FirstOrDefault(p => p.Status.ConstantId == statusId && p.Title.TitleId == titleId);
            //    if (un != null)
            //        break;


            //    un = new UserNotification();
            //    un.User = user;
            //    un.Title = title;
            //    un.Status = status;
            //    user.Notifications.Add(un);



            //} while (false);
            
        }

        private void DeleteNotification(Guid statusId, Guid titleId)
        {
            do
            {
                iZINE.Businesslayer.User user = DataContext.Users.Include("Notifications.Title").Include("Notifications.Status").Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.User.Identity.Name) == 0) as User;

                UserNotification un = user.Notifications.FirstOrDefault(p => p.status.StatusId == statusId && p.Title.TitleId == titleId);
                if (un == null)
                    break;

                user.Notifications.Remove(un);
               

            } while (false);
            
        }



    }
}
