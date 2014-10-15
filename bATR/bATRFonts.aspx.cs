using System;
using System.Web.UI.WebControls;
using System.Windows.Media;
using bATR.Config;

public partial class bATR_bATRFonts : System.Web.UI.Page
{

	protected void Page_Load(object sender, EventArgs e)
	{
		GetFonts();
	}



	public void GetFonts()
	{
		bATRConfigLogic bcl = new bATRConfigLogic();
		bATRConfig config = bcl.Config;

		System.Collections.Generic.ICollection<Typeface> typefaces = Fonts.GetTypefaces(config.FontPath);

		rptFonts.DataSource = typefaces;
		rptFonts.DataBind();
	}


	string previousFontName = string.Empty;
	protected void rptFonts_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
		{
			Typeface tf = (Typeface)e.Item.DataItem;
			Literal ltlFontName = (Literal)e.Item.FindControl("ltlFontName");
			Literal ltlFontWeight = (Literal)e.Item.FindControl("ltlFontWeight");
			Literal ltlFontStyle = (Literal)e.Item.FindControl("ltlFontStyle");
			Literal ltlFontStretch = (Literal)e.Item.FindControl("ltlFontStretch");

			string[] familyName = tf.FontFamily.Source.Split('#');
			string fontName = familyName[familyName.Length - 1].ToString();

			if (fontName != previousFontName)
			{
				ltlFontName.Text = fontName;
				previousFontName = fontName;
			}
			ltlFontWeight.Text = tf.Weight.ToString();
			ltlFontStyle.Text = tf.Style.ToString();
			ltlFontStretch.Text = tf.Stretch.ToString();
		}
	}



}
