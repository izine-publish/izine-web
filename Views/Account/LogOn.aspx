<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Web.MVC.Models.LogOnModel>"  %>
<%@ Register TagPrefix="izine" Namespace="iZINE.Web.Controls" %>
<%@ Register Assembly="iZINE.Web.Controls" Namespace="iZINE.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.loginHeader %></div></div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'>
                <%= Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.") %>
                <% using (Html.BeginForm()) { %>
                        <table cellpadding="2" cellspacing="2">
                        <tr>
                            <td width="100" style="width: 125px;"><label ><%=Resources.Resource.userNamelbl %></label></td>
                             <td>   
                                <%= Html.TextBox("username",null, new { @class = "username", style="width:201px;" })%>
                                </td>
                                <td width="100" style="width: 125px;">
                                <%= Html.ValidationMessage("username")%>
                                </td>
                        </tr>
                        <tr>
                            <td width="100" style="width: 125px;"><label><%=Resources.Resource.Passwordlbl %></label></td>
                            <td width="100" style="width: 125px;">
                                <%= Html.Password("Password", null, new { @class = "textbox", style="width:220px;" })%>
                            </td>
                            <td width="100" style="width: 125px;">
                                <%= Html.ValidationMessage("Password") %>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                             <td colspan="2"><input type="submit" value="LOGIN" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" /></td>
                        </tr>
                     </table>   
                <% } %>
            </div>
        </div>
        <div class='formBoxFooter'></div>
    </div>
</asp:Content>
