using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iZINE.Businesslayer;
using System.Web.Mvc;

namespace iZINE.Web.MVC.Models.Status
{
    public class Index
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

        public SelectList Groups
        {
            get;
            set;
        }

        public IList<Asset> Assets
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
