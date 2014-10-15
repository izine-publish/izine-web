<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" 
Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Models.DownloadModal>>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.DownloadsHeader %></div></div>
        <div class='formBoxBody'>
        <div class='formBoxBodyInner'>
         <%--<%if(Model != null && Model.Count > 0) {%>
             <%foreach(iZINE.Web.MVC.Models.DownloadModal don in Model) %>
             <%{ %>
                <div>
                    <a id='lName' href='<%= String.Format("/downloads.axd?id={0}", don.DownloadId)%>' ><%= Html.Encode(don.DownloadName)%></a>
                        <div style="padding-left: 15px">
                            <label><%=don.Description %></label>
                        </div>
                        <br />
                 </div>
             <%} %>
         <%} %>
        --%>
        <h2><b><%=Resources.Resource.comingSoon %></b></h2>
        </div></div>
       <div class='formBoxFooter'></div>
       </div>
    

</asp:Content>


