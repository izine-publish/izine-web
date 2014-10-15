<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="iZINE.Web.MVC.Extensions" %>
<ul>
	<li>
		<%if (ViewContext.RouteData.DataTokens.Count() > 0 || ViewContext.RouteData.DataTokens["area"] != null || ViewContext.Controller.ControllerContext.RequestContext.RouteData.Values["controller"].ToString().ToLower().CompareTo("setting") == 0)
	{%>
	<%= Html.ActionLink("home", "index", "home", new { area = "" },null)%>
		
		<%}
	else
	{ %>
		<%= Html.ActionLink("home", "index", "home", new { area = "" }, new { style = "font-weight: bold;" })%>
		<%} %>
		|</li>
	<%if (Page.User.IsInRole("0d629cba-f55c-461a-831e-58053db7189f"))
   {%>
	<% if (ViewContext.RouteData.DataTokens.Count() > 0 && ViewContext.RouteData.DataTokens["area"] != null && ViewContext.RouteData.DataTokens["area"].ToString().ToLower().CompareTo("adminuser") == 0)
   {%>
	<li>
		<%=Html.ActionLink("admin", "IndexAdmin", "AdminHome", new { area = "AdminUser" }, new { style = "font-weight: bold;" })%>|</li>
	<%}
   else
   { %>
	<li>
		<%=Html.ActionLink("admin", "IndexAdmin", "AdminHome", new { area = "AdminUser" },null)%>|</li>
	<%} %>
	<li>
	<%if (ViewContext.Controller.ControllerContext.RequestContext.RouteData.Values["controller"].ToString().ToLower().CompareTo("setting") == 0)
   {%>
	<%=Html.ActionLink(Resources.Resource.settings, "index", "setting", new { area = "" }, new { style = "font-weight: bold;" })%>
	<%}else{ %>
		<%=Html.ActionLink(Resources.Resource.settings, "index", "setting", new { area = "" }, null)%>
		<%} %>|</li>
	<%}%>
	<%if (Page.User.IsInRole("1313712B-7675-450A-A0D3-C774366DFE45"))
   { %>
	<li>
		<%=Html.TopMenuActionLink("admin", "KeyIndex", "KeyHome", new {area = "keyUser" })%>|</li>
	<%} %>
	<%  if (Request.IsAuthenticated)
	 {
	%>
	<li>
		<%= Html.TopMenuActionLink(Resources.Resource.logout, "logoff", "account", null)%></li>
	<%
		}
	 else
	 {
	%>
	<li>
		<%= Html.TopMenuActionLink("Log On", "logon", "account",null)%>
	</li>
	<%
		}
	%>
</ul>
