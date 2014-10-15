using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.Server
{
    // data transfer object
    public class TitleStatusDTO
    {
        public Guid titleid;
        public StatusDTO[] statuses;
    }
}
