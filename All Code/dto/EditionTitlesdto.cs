using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.Server
{
    // data transfer object
    public class EditionTitlesDTO
    {
        public Guid titleid;
        public ShelveDTO[] editions;
    }
}
