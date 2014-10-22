using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iZINE.Web.Server
{
    // data transfer object
    public class AssetDTO
    {
        public Guid assetid;
        public String name;
        public Guid? documentid;
        public String documentname;
        public Guid? titleid;
        public Guid? shelveid;
        
        public Guid? lockid;

        public Guid? typeid;
        public String type;

        public VersionInfo Head;
    }

    public class VersionInfo
    {
        public Guid? statusid;
        public DateTime? date;

        public Guid? versionid;
        public int? number;

        public UserDTO user;
    }
}
