using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iZINE.Web.Server
{
    // data transfer object
    public class UserDTO
    {
        public Guid userid;
        public String username;
        public String name;
    }
}
