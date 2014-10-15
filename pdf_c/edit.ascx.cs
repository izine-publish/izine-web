using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using iZINE.Web.Utils;
using System.IO;
using iZINE.Businesslayer;
using iZINE.Web;

namespace iZine.Web.PDF_C.Controls
{
    public partial class Edit : BaseAddEditUserControl
    {        
        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            NameTextBox.Text = Asset.Name;

            TitleLabel.Text = Title.Name;
            ShelveLabel.Text = Shelve.Name;            
        }

        public iZINE.Businesslayer.Asset Asset
        {
            get
            {
                iZINE.Businesslayer.Asset asset;

                if (ViewState["AssetId"] == null)
                {
                    asset = new iZINE.Businesslayer.Asset();
                    asset.AssetId = Guid.NewGuid();
                    asset.Active = true;

                }
                else
                {
                    Guid assetid = (Guid)ViewState["AssetId"];
                    asset = DataContext.Assets.FirstOrDefault(u => u.AssetId == assetid);
                }

                return (asset);
            }
            set
            {
                if (value == null)
                {
                    ViewState["AssetId"] = null;
                }
                else
                {
                    ViewState["AssetId"] = value.AssetId;
                }
            }
        }

        public iZINE.Businesslayer.Shelve Shelve
        {
            get
            {
                iZINE.Businesslayer.Shelve shelve;

                if (ViewState["ShelveId"] == null)
                {
                    return (null);
                }
                else
                {
                    Guid shelveid = (Guid)ViewState["ShelveId"];
                    shelve = DataContext.Shelves.FirstOrDefault(u => u.ShelveId == shelveid);
                }

                return (shelve);
            }
            set
            {
                if (value == null)
                {
                    ViewState["ShelveId"] = null;
                }
                else
                {
                    ViewState["ShelveId"] = value.ShelveId;
                }
            }
        }

        public iZINE.Businesslayer.Title Title
        {
            get
            {
                iZINE.Businesslayer.Title title;

                if (ViewState["TitleId"] == null)
                {
                    return (null);
                }
                else
                {
                    Guid titleid = (Guid)ViewState["TitleId"];
                    title = DataContext.Titles.FirstOrDefault(u => u.TitleId == titleid);
                }

                return (title);
            }
            set
            {
                if (value == null)
                {
                    ViewState["TitleId"] = null;
                }
                else
                {
                    ViewState["TitleId"] = value.TitleId;
                }
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {                
                iZINE.Businesslayer.Type type = DataContext.Constants.FirstOrDefault(t => t.ConstantId == Constant.PDF_C) as iZINE.Businesslayer.Type;

                if (type == null)
                {
                    throw new Exception("type not found");
                }

                iZINE.Businesslayer.Asset asset = this.Asset;
                
                asset.Name = NameTextBox.Text;
                asset.Date = DateTime.Now;
                asset.Title = Title;
                asset.Shelve = Shelve;
                asset.Type = type;
                asset.Active = true;

                int nextVersionNumber = GetAssetNextVersionNumber(asset.AssetId);

                if (FileUpload1.HasFile)
                {
                    iZINE.Businesslayer.Version version = new iZINE.Businesslayer.Version();

                    version.VersionId = Guid.NewGuid();
                    version.Asset = asset;
                    version.Date = DateTime.Now;
                    version.Number = nextVersionNumber;                    

                    iZINE.Businesslayer.User user = DataContext.Users.FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.Current.User.Identity.Name) == 0) as iZINE.Businesslayer.User;
                    version.User = user;

                    version.Active = true;
                    version.Filename = GetSelectedFileName();
                    asset.Filename = GetSelectedFileName();

                    ValidateFileType();
                    String assetsDir = System.Configuration.ConfigurationManager.AppSettings["assetsDir"];
                    FileUpload1.PostedFile.SaveAs(Path.Combine(assetsDir, String.Format("{0}.pdf", version.VersionId.ToString())));

                    asset.Version = version;
                }
                else
                {
                    if (nextVersionNumber == 1)
                    {
                        // it's the first version and we need a file to be uploaded
                        throw new Exception("File is empty");
                    }
                }

                

                // 
                // save all changes in a transaction
                //
                DataContext.SaveChanges();

                ControlSaved();
            }
            catch (Exception exc)
            {
                log.Error(exc);
                throw;
            }
        }

        private string GetSelectedFileName()
        {
            string fullName = FileUpload1.PostedFile.FileName;

            if (fullName.Contains("\\"))
            {
                return fullName.Substring(fullName.LastIndexOf("\\") + 1);
            }
            else
            {
                return fullName;
            }
        }

        private void ValidateFileType()
        {
            string fileName = GetSelectedFileName();

            fileName = fileName.ToLower();

            if (!fileName.EndsWith(".pdf"))
            {
                throw new Exception("Invalid file type. PDF expected");
            }
        }

        private int GetAssetNextVersionNumber(Guid assetId)
        {
            int nextVersionNumber = 1;

            var versionNumbers = from v in DataContext.Versions
                                 where v.Asset.AssetId == assetId
                                 select v.Number;

            if (versionNumbers.Count() > 0)
            {
                nextVersionNumber = versionNumbers.Max().Value + 1;
            }

            return nextVersionNumber;
        }        
    }
}