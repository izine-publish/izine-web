using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Models
{
    public class TitleStatusModel
    {
        public Guid StatusId { get; set; }
        public string Name { get; set; }
        public Guid TitleId { get; set;}
        public bool Layout { get; set; }
        public bool text { get; set;}
    }
}
