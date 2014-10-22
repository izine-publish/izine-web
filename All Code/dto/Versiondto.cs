using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.Server
{
    public class VersionDTO
    {
        public Guid? versionId;
        public byte[] data;
        public bool isCompressed;
        public int? versionNumber;
    }
}
