<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/AdminUser/Views/Shared/AdminUser.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.adminHeader%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
            <%=Resources.Resource.welcomeAdministratorPanel%>
              
             </div></div>
       <div class='formBoxFooter'></div>
       </div>
</asp:Content>
