using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using iZINE.Businesslayer;

namespace iZINE.Web.MVC.Areas.AdminUser.Models
{
    public class TitleStatusModel
    {
        public Guid StatusId { get; set; }
        public string Name { get; set; }
		public string State { get; set; }
        public Guid TitleId { get; set; }
        public bool Layout { get; set; }
        public bool text { get; set; }
    }

	public class EditTitleStatusModel
	{
		public Guid StatusId { get; set; }

		[Required]
		public string Name { get; set; }

		public Guid ?StateId { get; set; }
		public List<StatusState> States { get; set; }
		
	}
}
