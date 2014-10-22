using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Areas.AdminUser.Models
{
    public class TitleModel
    {
        public Guid TitleId { get; set; }
        public string Name { get; set; }
        public bool Seleceted { get; set; }
    }
}
