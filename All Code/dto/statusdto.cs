using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.Server
{
    // data transfer object
    public class StatusDTO
    {
        public Guid statusId;
        public string statusName;
        public bool layout;
        public bool text;
		public Guid stateId;
    }
}
