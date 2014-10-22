using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iZINE.Businesslayer;

public partial class DocumentPagePreview : System.Web.UI.UserControl
{
    public string Number
    {
        get
        {
            return sNumber.InnerText;
        }
        set
        {
            sNumber.InnerText = value;
        }
    }

    private static int MaxPageNameLength = 36;

    public string Name
    {
        get
        {
            return sName.InnerText;
        }
        set
        {            
            sName.Attributes.Add("title", value);

            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > MaxPageNameLength)
                {
                    sName.InnerText = value.Substring(0, MaxPageNameLength);
                }
                else
                {
                    sName.InnerText = value;
                }
            }

            sNamePopUp.InnerText = value;
        }
    }

    public string PageImageUrl
    {
        get
        {
            return PageImage.ImageUrl;
        }
        set
        {
            PageImage.ImageUrl = value;            
        }
    }

    public string HyperLinkPagePDFUrl
    {
        get
        {
            return HyperLinkPDFPage.NavigateUrl;
        }
        set
        {
            HyperLinkPDFPage.NavigateUrl = value;
        }
    }

    public bool CheckImageVisible
    {
        get
        {
            return CheckImage.Visible;
        }
        set
        {
            CheckImage.Visible = value;
        }
    }

    public Guid StatusID
    {
        get;
        set;
    }

    public DocumentPagePreviewData Data
    { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnPreRender(EventArgs e)
    {
        if (Data.PageId != Guid.Empty)
        {
            this.Name = Data.Name;
            this.Number = Data.Number.ToString();
            this.PageImageUrl = string.Format("thumbnail.axd?id={0}", this.Data.PageId);
            this.HyperLinkPagePDFUrl = string.Format("pdf.axd?id={0}", this.Data.VersionId);
            this.StatusID = this.Data.StatusId;
            ThumbnailLinkButton.NavigateUrl = string.Format("image.axd?id={0}", this.Data.PageId);
            ThumbnailLinkButton.Attributes.Add("title", Data.Number.ToString() + ". " + Data.Name);

            // this is only for testing pages layout
            //if (this.Data.Type.ConstantId == Constant.LeftPageType)
            //{
            //    this.PageImage.ToolTip = "Left Page";
            //}
            //else if (this.Data.Type.ConstantId == Constant.RightPageType)
            //{
            //    this.PageImage.ToolTip = "Right Page";
            //}
            //else if (this.Data.Type.ConstantId == Constant.UnisexPageType)
            //{
            //    this.PageImage.ToolTip = "Unisex Page";
            //}

            base.OnPreRender(e);

            if (new Guid("22F8ACBD-8E7C-4437-A767-E5773DB76083").CompareTo(StatusID) == 0)
            {
                //
                //Panel PagePanel=e.Item.FindControl("PagePanel") as Panel;
                //PagePanel.BackColor = System.Drawing.Color.FromArgb(0xEF, 0xF2, 0xBF);

                CheckImageVisible = true;
            }
            else
            {
                CheckImageVisible = false;
            }
        }
        else
        {
            dHeading.Visible = false;
            ThumbnailLinkButton.Visible = false;
            CheckImage.Visible = false;
            HyperLinkPDFPage.Visible = false;
            ImagePanel.Visible = false;
        }
    }
}
