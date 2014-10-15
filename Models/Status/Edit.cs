using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iZINE.Businesslayer;
using System.Web.Mvc;

namespace iZINE.Web.MVC.Models.Status
{
    public class Edit
    {
        public SelectList Titles
        {
            get;
            set;
        }

        public SelectList Shelves
        {
            get;
            set;
        }

        public SelectList Status
        {
            get;
            set;
        }

        public Asset Asset
        {
            get;
            set;
        }

        public IList<StatusModel> Assetlist
        {
            get;
            set;
        }
 
    }
}
