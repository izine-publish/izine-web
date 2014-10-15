<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
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
