using System;
using System.Web;
using System.Web.Security;
using bATR.Config;

public partial class bATR_Masters_default : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Session["bATRAuth"] != null && Session["bATRAuth"].ToString() == "GoOn")
			mvContent.SetActiveView(vContent);
		else
			mvContent.SetActiveView(vLogin);
	}



	/// <summary>
	/// Authenticates the user and binds data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnLogin_Click(object sender, EventArgs e)
	{
		if (txtLogin.Text.Length > 0 && txtPasswd.Text.Length > 0)
		{
			bATRConfigLogic bcl = new bATRConfigLogic();
			bATRConfig config = bcl.Config;

			if (txtLogin.Text == config.Admin.Username && txtPasswd.Text == config.Admin.Password)
			{
				Session.Add("bATRAuth","GoOn");
				mvContent.SetActiveView(vContent);
			}
		}
	}


	/// <summary>
	/// Go away! Log off
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLogoff_Click(object sender, EventArgs e)
	{
		Session.Remove("bATRAuth");
		Response.Redirect(Request.Url.AbsoluteUri);
	}

}
