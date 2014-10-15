using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iZINE.Web.Server
{
    // data transfer object
    public class TaskDTO
    {
        public Guid taskid;
        public Guid assetid;
        public String name;
        public String description;
        public DateTime date;
        public ConstantDTO status;
        public UserDTO user;
    }
}
