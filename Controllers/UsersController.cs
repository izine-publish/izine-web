//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Controllers/UsersController.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-01 12:44:49 +0530 (Mon, 01 Feb 2010) $

//    $Revision: 797 $

//    $Author: prakash.bhatt $

//    Description: Controller class for users

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using iZINE.Web.MVC.Models;
using MvcPaging;
using System.Security.Cryptography;
using System.Text;

namespace iZINE.Web.MVC.Controllers
{
    public class UsersController : Controller
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
        public ActionResult Index()
        {
            ViewData["Organisation"] = GetOrganisations(null);
            return View("default");
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
        [Authorize]
        public ActionResult GetList(Guid? Organisation, int? page)
        {
            ViewData["Organisation"] = GetOrganisations(Organisation);
            ViewData["OrgSelected"] = Organisation;
            if (Organisation.HasValue)
            {
                int pageIndex = 0;
                if (page.HasValue)
                    pageIndex = page.Value -1;

                return View("default", GetUsers(Organisation.Value).ToPagedList(pageIndex, 5));
            }
            else
            {
                return View("default");
            }


        }
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetJsonList(Guid? Organisation, int? page)
        {
           
            int pageIndex = 0;
            if (page.HasValue)
                pageIndex = page.Value - 1;

            var userList = GetUsers(Organisation.Value);

            JsonResult result = new JsonResult();
            result.Data = new { count = userList.Count(), items = (from i in userList orderby i.UserId descending select i).Skip(pageIndex * 5).Take(5) };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }

        protected IEnumerable<UserModel> GetUsers(Guid organizationId)
        {
            IEnumerable<UserModel> users = from u in DataContext.Users.Include("Role").Include("Membership")
                                           where u.Organization.OrganizationId == organizationId && u.Active == true
                                           orderby u.LastName, u.FirstName
                                           select new UserModel
                                           {
                                               FirstName = u.FirstName,
                                               MiddleName = u.MiddleName,
                                               LastName =  u.LastName,
                                               RoleId = u.Role.RoleId, 
                                               RoleName = u.Role.Name, 
                                               UserId = u.UserId, 
                                               UserName = u.Membership.Username, 
                                               EmailAddress = u.Membership.Email,
                                               LockedOut = u.Membership.LockedOut
                                           };



            return users;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteLinkHandler(Guid? id)
        {
            var result = new JsonResult();


            iZINE.Businesslayer.User user = DataContext.Users.Include("Membership").FirstOrDefault(u => u.UserId == id);
            user.Active = false;
            DataContext.SaveChanges();

            result.Data = new { Success = "success" };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;


        }
        [Authorize]
        public ActionResult Create(Guid? organisation)
        {
            iZINE.Businesslayer.User user = new iZINE.Businesslayer.User();
            user.Membership = new Membership();

            ViewData["Organisation"] = GetOrganisations(organisation);

            IEnumerable<TitleModel> titleList = from o in DataContext.Titles
                                                where o.Active == true
                                                orderby o.Name
                                                select new TitleModel { TitleId = o.TitleId, Name = o.Name };

            ViewData["Titles"] = titleList.ToList();

            ViewData["Rol"] = GetRoles(null);

            return View("EditUser", user);
        }

        [Authorize]
        public ActionResult EditUserView(Guid? userId)
        {
            IEnumerable<TitleModel> titleList = from o in DataContext.Titles
                                             where o.Active == true
                                             orderby o.Name
                                             select new TitleModel { TitleId = o.TitleId, Name = o.Name };

           
            iZINE.Businesslayer.User user = GetUserById(userId);

            var titles = user.Titles;

            List<TitleModel> ltitle= new List<TitleModel>();

            foreach (TitleModel item in titleList)
            { 
                // get title
                var title = (from o in DataContext.Titles
                             where o.TitleId == item.TitleId
                             select o).FirstOrDefault();

                item.Seleceted = titles.Contains(title);
                ltitle.Add(item);
                
            }

            

            ViewData["Organisation"] = GetOrganisations(user.Organization.OrganizationId);
            ViewData["Titles"] = ltitle.ToList();
            ViewData["Rol"] = GetRoles(user.Role.RoleId);

            return View("EditUser", user);
        }

        private SelectList GetRoles(Guid? roleSelected)
        {
            SelectListItem obj = new SelectListItem();
            obj.Value = "";
            obj.Text = "         ";

            SelectListItem obj1 = new SelectListItem();
            obj1.Value = "0d629cba-f55c-461a-831e-58053db7189f";
            obj1.Text = "Administrator";

            SelectListItem obj2 = new SelectListItem();
            obj2.Value = "1313712b-7675-450a-a0d3-c774366dfe45";
            obj2.Text = "Key User";

            SelectListItem obj3 = new SelectListItem();
            obj3.Value = "8b5be9fb-57ac-4c77-b00e-4c366b0e47f5";
            obj3.Text = "User";
            
            List<SelectListItem> selectList = new List<SelectListItem>();
            selectList.Add(obj);
            selectList.Add(obj1);
            selectList.Add(obj2);
            selectList.Add(obj3);


            var roleList = new SelectList(selectList.ToList(),"Value","Text", roleSelected);
            return roleList;
             
      
        }

        public ActionResult EditUser(Guid? userId)
        {
            Guid organizationId = Guid.NewGuid();
            try
            {
               
                if(Request.Form["button"] == null)
                {
                    iZINE.Businesslayer.User user = GetUserById(userId);
                    organizationId = new Guid(Request.Form["Organisation"]);
                   
                    TryUpdateModel(user, new[] { "FirstName", "MiddleName", "LastName" });
                    TryUpdateModel(user.Membership, new[] { "Email", "Username" });

                    
                    iZINE.Businesslayer.Organization organization = DataContext.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);
                    user.Organization = organization;

                    Guid roleid;
                    iZINE.Web.Utils.Common.GuidTryParse(Request.Form["Rol"].ToString(), out roleid);

                    Role role = DataContext.Roles.FirstOrDefault(r => r.RoleId == roleid);
                    user.Role = role;


                    user.Membership.LockedOut = Request.Form["LockedOut"].ToString() == "false" ? false : true;
                    user.Membership.FailedPasswordAttemptCount = 0;

                    if (!String.IsNullOrEmpty(Request.Form["Password"].ToString()))
                    {
                        // set new password
                        user.Membership.Salt = CreateSalt(12);
                        user.Membership.Password = hashPassword(Request.Form["Password"].ToString(), user.Membership.Salt);
                    }

                    if (user.EntityState == System.Data.EntityState.Detached)
                        DataContext.AddToUsers(user);

                    DataContext.SaveChanges();

                    user.Titles.Load();

                    var titleList = from o in DataContext.Titles
                                                        where o.Active == true
                                                        orderby o.Name
                                                        select o;

                    //since all variables are dynamically bound you must load your DB into strings in a for loop as so:
                    
                    foreach (Title title in titleList)
                    {


                        var checkbox = Request.Form["" + title.TitleId];
                        // the reason you check for false because the Html checkbox helper does some kind of freaky thing for value true: it makes the string read "true, false"
                        if (checkbox != "false")
                        {
                            user.Titles.Add(title);
                        }
                        else
                        {
                            // delete
                            user.Titles.Remove(title);
                        }

                    }

                    DataContext.SaveChanges();

                }  
            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }
            
            ViewData["Organisation"] = GetOrganisations(null);
            return RedirectToAction("getlist", new { organisation = organizationId, page = 1 });
        }

        public iZINE.Businesslayer.User GetUserById(Guid? userid)
        {
            
            iZINE.Businesslayer.User user;

            if (!userid.HasValue || userid.Value == Guid.Empty)
            {
                user = new iZINE.Businesslayer.User();

                user.Membership = new iZINE.Businesslayer.Membership();
                user.UserId = Guid.NewGuid();
                user.Membership.UserId = user.UserId;
                user.Active = true;

            }
            else
            {
                user = DataContext.Users.Include("Role").Include("Membership").FirstOrDefault(u => u.UserId == userid);

                user.OrganizationReference.Load();
                user.MembershipReference.Load();
                user.Titles.Load();
            }

            return (user);
            
        }

        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        private static string hashPassword(string password, string salt)
        {
            string retValue = string.Empty;
            SHA1 sha1 = SHA1.Create();
            string tmpPassword = salt + password;
            byte[] bytePassword = sha1.ComputeHash(Encoding.Unicode.GetBytes(tmpPassword));
            // Releases all resources used by the System.Security.Cryptography.HashAlgorithm. 
            sha1.Clear();
            retValue = Convert.ToBase64String(bytePassword);
            return retValue;
        }

        

    }
}
