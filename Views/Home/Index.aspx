<%@ Page Language="C#" MasterPageFile="~/Views/Shared/AdminSite.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
 
    <div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.homeHeader %></div></div>
        <div class='formBoxBody'>
        <div class='formBoxBodyInner'>
             
            <table width="100%" height="300px;>
                <tr>
                    <td colspan="2" class="Heading1"></td>
                </tr>
            </table>
 
    </div>
    </div>
     <div class='formBoxFooter'></div>
    </div>
    
</asp:Content>
