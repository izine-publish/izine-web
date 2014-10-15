<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<iZINE.Web.MVC.Models.Status.Index>" %>
<table width="810px" cellpadding="1" cellspacing="0" class="status">
    <thead>
        <tr style="height: 15px;">
             <th align="left" style="width: 60px; border-right: solid 1px black; padding: 3px 5px; "><%= Resources.Resource.status%></th>
             <th align="left" style="width: 60px; border-right: solid 1px black; padding: 3px 5px;" width="100px"><%= Resources.Resource.pageNumber%></th>
             <th align="left" style="width: 250px; border-right: solid 1px black; padding: 3px 5px;"><%= Resources.Resource.document%></th>
             <th align="left" style="width: 250px; padding: 3px 5px;">In gebruik door</th>
             <th align="left" style="width: 20px; border-right: solid 1px black; padding: 3px 5px; width: 20px;"></th>
             <th align="left" style="width: 150px; border-right: solid 1px black; padding: 3px 5px;">Laatst gewijzigd</th>
             <th align="left" style="width: 50px; border-right: solid 1px black; padding: 3px 5px;"><%= Resources.Resource.versionNumber%></th>
             <th align="left" style="width: 50px; padding: 3px 5px;">PDF</th>
        </tr>
    </thead>
    <tbody>
        <% if (Model.Assetlist.Count() > 0)
           { %>
        <% foreach (iZINE.Web.MVC.Models.StatusModel a in Model.Assetlist)
           {  %>
        <tr>
            <td style="width: 60px;  padding: 1px 5px 0px 5px;">
                <div style="width: 15px; height: 10px; margin: 0px 12px;">
                <ul  style="color:green; list-style-type: disc; list-style-position: inside; font-size: 2.4em; padding: 2px 2px; overflow: hidden; position: relative; top: -16px; left: 20x;" >
			        <li></li>
		        </ul>
		        </div>
            </td>
            <td style="width: 60px; overflow: hidden; padding: 1px 5px 0px 5px;"><%=a.PageNumber%></td>
            <td style="width: 250px; overflow: hidden;  padding: 1px 5px 0px 5px;"><%=Html.ActionLink(a.Name, "edit", new { assetid = a.DocumentId }, new {@class="popup" })%></td>
            <td style="width: 250px; overflow: hidden;  padding: 1px 5px 0px 5px;"><%= String.Format("{0} {1} {2}", a.UserFName, a.UserMName, a.UserLName)%></td>
            <td style="width: 20px; overflow: hidden; padding: 1px 5px 0px 5px;"><% if (a.IsLocked)
                   { %><img src="/images/lock.png" width="16" height="16" alt="Lock" />
                <% } %>
            </td>
            <td style="width: 150px; overflow: hidden; padding: 1px 5px 0px 5px;"><%=a.Date.Value.ToString("dd-MM-yyyy")%></td>
            <td style="width: 50px; overflow: hidden; padding: 1px 5px 0px 5px;">#<%=a.VersionNumber%></td>
            <td style="width: 50px; overflow: hidden; padding: 1px 5px 0px 5px;"><a href="pdf.axd?id=<%=a.VersionId %>"><img src="/App_Themes/green/images/pdf_icon.png" width="16" height="16" alt="Status" /></a></td>
        </tr>
        <% foreach (iZINE.Web.MVC.Models.statusAssignment al in a.AssignmentsList)
           { %>
        <tr style="color:#893b96;font-size:11px;font-weight:bold;font:arial,helvetica;">
            <td><img style="border-style: none" alt="test" src="<%=al.url %>"/></td>
            <td style="font-size: 11px; font-style:italic;"><%=al.Name%></td>
            <td><%=al.Date.Value.ToShortDateString()%></td>
            <td><%= String.Format("{0} {1} {2}", al.UserFName, al.UserMName, al.UserLName)%></td>
            <td>#<%=al.VersionNumber%></td>
        </tr>
        <% } %>
        <tr>
            <td colspan="8">
                <div style="border-top: 1px solid rgb(190, 204, 204); height: 1px;" width="100%"> </div>
            </td>
        </tr>
        <% } %>

        <% } else {%>
            <tr>
                <td colspan="8">Geen data</td>
            </tr>
        <% } %>
    </tbody>
</table>
