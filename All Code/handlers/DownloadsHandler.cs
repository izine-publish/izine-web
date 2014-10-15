using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iZINE.Businesslayer;
using System.Configuration;
using System.IO;

namespace iZINE.Web.Handlers
{
    public class DownloadsHandler : IHttpHandler
    {
        public DownloadsHandler()
        {
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["id"] == null)
                return;

            Guid id;

            if (!iZINE.Web.Utils.Common.GuidTryParse(context.Request["id"], out id))
                return;

            var download = Downloads.GetByID(id);

            if (download == null)
                return;

            string filename = context.Server.MapPath(ConfigurationManager.AppSettings["downloadsVirtualDir"] + "/" + download.DownloadId.ToString());
            if (File.Exists(filename))
            {
                Stream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                context.Response.Clear();
                context.Response.ContentType = download.MimeType;
                context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}\"", download.Filename));
                context.Response.BufferOutput = false;

                context.Response.StatusCode = 200;

                byte[] buffer = new byte[4096];

                int count;

                while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, count);
                }

                context.Response.Flush();
                inputStream.Close();
            } else {
                context.Response.StatusCode = 404;
            }
        }

        #endregion
    }
}