using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iZINE.Web.MVC.Areas.KeyUser.Models
{
    public class UnLockModel
    {
        public Guid assetId { get; set; }
        public Guid? lockId { get; set; }
        public Guid? userId { get; set; }
        public string assetName { get; set; }
        public string titleName { get; set; }
        public string docName { get; set; }
        public string userName { get; set; }
        public DateTime date { get; set; }
        public string applicationName { get; set; }
    }
}
