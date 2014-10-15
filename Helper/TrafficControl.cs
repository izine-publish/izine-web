using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace iZINE.Web.Controls {
/// <summary>
/// Summary description for TrafficControl
/// </summary>
public class TrafficControl:WebControl
{
    public TrafficControl()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    private Image _image;

    public Guid StatusId {
 
        get
        {
            if (ViewState["StatusId"] == null)
                return Guid.Empty;

            Guid _statusId = (Guid)ViewState["StatusId"]; ;
            return (_statusId);
        }
        set
        {
            ViewState["StatusId"]=value;
        }
    }

    protected override void CreateChildControls()
    {
        _image = new Image();
        
        if (new Guid("0b75fc2d-eda3-4a56-a6ea-27dc9970e80b").CompareTo(StatusId) == 0)
        {
            // eindredactie
            _image.ImageUrl = "~/images/status3.gif";
            _image.AlternateText = "Eindredactie";            
        }
        else if (new Guid("a350e418-e5b0-4fe5-92ee-66d9814e29c2").CompareTo(StatusId) == 0)
        {
            // vormgeving
            _image.ImageUrl = "~/images/status1.gif";
            _image.AlternateText = "Vormgeving";
        }
        else if (new Guid("69c76023-3cf2-4a58-90d7-ad410435f47f").CompareTo(StatusId) == 0)
        {
            // redactie
            _image.ImageUrl = "~/images/status2.gif";
            _image.AlternateText = "Redactie";
        }
        else if (new Guid("22f8acbd-8e7c-4437-a767-e5773db76083").CompareTo(StatusId) == 0)
        {
            // definitief
            _image.ImageUrl = "~/images/status4.gif";
            _image.AlternateText = "Definitief";
        }

        _image.ToolTip = _image.AlternateText;
    }

    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        _image.RenderControl(writer);
    }
}
}
