using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Models
{
    public class NotificationModel
    {
        public Guid TitleId { get; set; }
        public Guid StatusId { get; set; }
        public string TitleName { get; set; }
        public string StatusName { get; set; }
    }
}
