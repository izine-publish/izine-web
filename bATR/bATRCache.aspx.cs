using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using bATR.CacheMonitor;
using bATR.Config;

public partial class bATR_bATRCache : System.Web.UI.Page
{

	protected void Page_Load(object sender, EventArgs e)
	{
		Bind();
	}

	/// <summary>
	/// Shows the cache
	/// </summary>
	private void Bind()
	{
		if (HttpRuntime.Cache[bATRConfig.CachedItemsCacheKey] != null)
		{
			Dictionary<string, CacheEntry> cachedItems = (Dictionary<string, CacheEntry>)HttpRuntime.Cache[bATRConfig.CachedItemsCacheKey];

			if (cachedItems != null && cachedItems.Count > 0)
			{
				rptCacheContents.DataSource = cachedItems;
				rptCacheContents.DataBind();
			}
		}
	}


	private int totalItemCount = 0;
	private long totalItemSize = 0;
	protected void rptCacheContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			KeyValuePair<string, CacheEntry> row = (KeyValuePair<string, CacheEntry>)e.Item.DataItem;

			Literal ltlDate = (Literal)e.Item.FindControl("ltlDate");
			Literal ltlTimeLeft = (Literal)e.Item.FindControl("ltlTimeLeft");
			Literal ltlItem = (Literal)e.Item.FindControl("ltlItem");
			Literal ltlSize = (Literal)e.Item.FindControl("ltlSize");

			ltlDate.Text = row.Value.TimeCached.ToString();
			ltlTimeLeft.Text = row.Value.TimeCached.AddMinutes(row.Value.CacheTime).Subtract(DateTime.Now).ToString().Substring(0, 11);
			ltlItem.Text = row.Value.Key;
			ltlSize.Text = Math.Round((row.Value.Size / 1024.0), 2).ToString() + "Kb";

			totalItemCount++;
			totalItemSize = totalItemSize + row.Value.Size;
		}
		else if (e.Item.ItemType == ListItemType.Footer)
		{
			Literal ltlItemCount = (Literal)e.Item.FindControl("ltlItemCount");
			Literal ltlTotalSize = (Literal)e.Item.FindControl("ltlTotalSize");

			ltlItemCount.Text = totalItemCount.ToString();
			ltlTotalSize.Text = Math.Round((totalItemSize / 1024.0), 2).ToString() + "Kb";
		}
	}
}
