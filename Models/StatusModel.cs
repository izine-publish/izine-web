//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Models/StatusModel.cs $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: Model class for status

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;


namespace iZINE.Web.MVC.Models
{
    public class StatusModel : global::System.Data.Objects.DataClasses.EntityObject
    {
        public Guid DocumentId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        public Guid StatusId
        {
            get;
            set;
        }

        public Guid VersionId
        {
            get;
            set;
        }

        public DateTime? Date
        {
            get;
            set;
        }

        public int? VersionNumber
        {
            get;
            set;
        }

        public int? PageNumber
        {
            get;
            set;
        }
		public bool IsLocked
		{
			get;
			set;
		}
		public string LockedBy { get; set; }
        public string UserFName { get; set; }
        public string UserMName { get; set; }
        public string UserLName { get; set; }

        public List<iZINE.Web.MVC.Models.statusAssignment> AssignmentsList
        {
            get;
            set;
        }
    }

    public class statusAssignment
    {
        public string Name { get; set; }
        public int? VersionNumber { get; set; }
        public Guid? statusid { get; set; }
   
        public DateTime? Date
        {
            get;
            set;
        }
        public string url { get; set; }
        public string UserFName { get; set; }
        public string UserMName { get; set; }
        public string UserLName { get; set; }
    }
    
}
