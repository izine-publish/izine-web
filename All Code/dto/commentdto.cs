using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iZINE.Web.Server
{
    // data transfer object
    public class CommentDTO
    {
        public Guid assetid;
        public String name;
        public String comment;
        
        public Guid? statusid;
        public String status;
        
        public Guid? userid;
        public String username;
        public DateTime? date;

        public Guid? versionid;
        public int? number;
    }
}
