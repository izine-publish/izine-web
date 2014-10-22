using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Models
{
    public class C_PdfModel
    {
        public Guid DocumentId
        {
            get;
            set;
        }
        public string titleName
        {
            get;
            set;
        }

        public string sheleveName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public Guid StatusId
        {
            get;
            set;
        }

        public Guid titileId
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
    }
}
