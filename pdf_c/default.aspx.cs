using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Collections.Generic;
using iZINE.Businesslayer;
using iZINE.Web.Utils;
using System.Web.Services;
using System.Collections;

namespace iZine.Web.PDF_C
{
    public partial class Default : BasePage
    {        
        protected void Page_Load(object sender, EventArgs e)
        {           
            StatusMultiView.SetActiveView(DefaultStatusView);

            if (!IsPostBack)
            {
                AssetsGridView.DataSource = GetAssets();
                AssetsGridView.DataBind();
            }

            AssetsGridView.RowEditing+=new GridViewEditEventHandler(AssetsGridView_RowEditing);
            AssetsGridView.RowDeleting+=new GridViewDeleteEventHandler(AssetsGridView_RowDeleting);
        }

        private IEnumerable GetAssets()
        {
            Guid shelveId;

            if (!iZINE.Web.Utils.Common.GuidTryParse(ShelvesDropDownList.SelectedValue, out shelveId))
            {
                return (null);
            }

            var assetList = from a in DataContext.Assets.Include("Version").Include("Version.User")
                            where a.Shelve.ShelveId == shelveId && a.Active == true && a.Type.ConstantId == Constant.PDF_C
                            orderby a.Version.Date descending
                            select a;

            return assetList;
        }

        protected void AssetsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid assetId = (Guid)AssetsGridView.DataKeys[e.RowIndex]["AssetId"];
            
            iZINE.Businesslayer.Asset asset = DataContext.Assets.Include("Version").FirstOrDefault(x => x.AssetId == assetId);

            asset.Active = false;

            DataContext.SaveChanges();

            AssetsGridView.DataSource = GetAssets();
            AssetsGridView.DataBind();
        }

        protected void AssetsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid assetId = (Guid)AssetsGridView.DataKeys[e.NewEditIndex]["AssetId"];

            iZINE.Businesslayer.Asset asset = DataContext.Assets.Include("Title").Include("Shelve").FirstOrDefault(x => x.AssetId == assetId);
            EditControl.Asset = asset;
            EditControl.Shelve = asset.Shelve;
            EditControl.Title = asset.Title;
            MultiView.SetActiveView(EditView);
        }


        [WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static AjaxControlToolkit.CascadingDropDownNameValue[] GetShelves(string knownCategoryValues, string category)
        {
            System.Collections.Specialized.StringDictionary kv = AjaxControlToolkit.CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);

            Guid titleid;
            if (!iZINE.Web.Utils.Common.GuidTryParse((string)kv["titles"], out titleid))
                return (new AjaxControlToolkit.CascadingDropDownNameValue[] { new AjaxControlToolkit.CascadingDropDownNameValue(knownCategoryValues, "test") });

            // convert to Linq to Objects using AsEnumerable. Creating a subquery.
            IEnumerable<AjaxControlToolkit.CascadingDropDownNameValue> values = from s in
                                                                                    (from s in DataContext.Shelves
                                                                                     where s.Title.TitleId == titleid
                                                                                     orderby s.Name
                                                                                     select s
                                                                                     ).AsEnumerable()
                                                                                select new AjaxControlToolkit.CascadingDropDownNameValue
                                                                                {
                                                                                    value = s.ShelveId.ToString(),
                                                                                    name = s.Name
                                                                                };
            return (values.ToArray());
        }
        
        
        protected void EditControl_Cancel(object sender, EventArgs e)
        {
            MultiView.SetActiveView(ListView);
        }

        protected void AssetsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            AssetsGridView.PageIndex = e.NewPageIndex;

            AssetsGridView.DataSource = GetAssets();
            AssetsGridView.DataBind();
        }

        protected void EditControl_Save(object sender, EventArgs e)
        {
            // update gridview
            MultiView.SetActiveView(ListView);

            StatusMultiView.SetActiveView(SavedStatusView);

            AssetsGridView.DataSource = GetAssets();
            AssetsGridView.DataBind();
        }

        // Source: http://intrepidnoodle.com/articles/24.aspx
        public T FindControl<T>(string id) where T : Control
        {
            // We know the control we're searching for isn't the Page itself,
            // so we use the more performant FindChildControl to search.
            return FindChildControl<T>(Page, id);
        }

        public T FindControl<T>(Control startingControl, string id) where T : Control
        {
            return ControlFinder.FindControl<T>(startingControl, id);
        }

        public T FindChildControl<T>(Control startingControl, string id) where T : Control
        {
            return ControlFinder.FindChildControl<T>(startingControl, id);
        }

        protected void CreateButton_Click(object sender, EventArgs e)
        {
            Guid titleId;
            if (!iZINE.Web.Utils.Common.GuidTryParse(TitlesDropDownList.SelectedValue, out titleId))
                return;
            
            EditControl.Title = Titles.GetById(titleId);

            Guid shelveId;
            if (!iZINE.Web.Utils.Common.GuidTryParse(ShelvesDropDownList.SelectedValue, out shelveId))
                return;
            
            EditControl.Shelve = Shelves.GetById(shelveId);
            
            MultiView.SetActiveView(EditView);
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static AjaxControlToolkit.CascadingDropDownNameValue[] GetTitles(string knownCategoryValues, string category)
        {
            iZINE.Businesslayer.User user = DataContext.Users.Include("Titles").FirstOrDefault(u => u.Membership.Username.CompareTo(HttpContext.Current.User.Identity.Name) == 0) as User;
            IEnumerable<AjaxControlToolkit.CascadingDropDownNameValue> values = from c in user.Titles
                                                                                where c is Title
                                                                                orderby c.Name
                                                                                select new AjaxControlToolkit.CascadingDropDownNameValue
                                                                                {
                                                                                    value = c.TitleId.ToString(),
                                                                                    name = c.Name
                                                                                };

            return (values.ToArray());
        }

        protected void ShelvesDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AssetsGridView.DataSource = GetAssets();
            AssetsGridView.DataBind();
        }
    }
}