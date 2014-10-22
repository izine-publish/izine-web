using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Areas.KeyUser.Models
{
    public class DownloadModal
    {
        public Guid DownloadId { get; set; }
        public string DownloadName { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
    }
}
