using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.Server
{
    // data transfer object
    public class LockDTO
    {
        public Guid lockid;
        public String applicationname;
        public String documentname;
        public Guid assetid;
        public Guid documentid;
        public Guid userid;
        public String username;
    }
}
