<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.KeyUser.Models.DownloadModal>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.DownloadsHeader %></div></div>
        <div class='formBoxBody'>
        <div class='formBoxBodyInner'>
      
      <%if(Model != null && Model.Count > 0) {%>
      <table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">
            <thead>
            <tr style="background-color: #ffffff;">
                <th align="left">                                                
                    <%=Resources.Resource.name %>
                </th>
                <th align="left">                                                
                    <%=Resources.Resource.date %>
                </th>
                <th align="left">                                                
                    <%=Resources.Resource.file%>
                </th>
                <th align="right">                                                
                   
                </th>
            </tr>
            </thead>
            <tbody>
             <%foreach(iZINE.Web.MVC.Areas.KeyUser.Models.DownloadModal don in Model) %>
             <%{ %>
                <tr>
                <td>
                    <%= Html.Encode(don.DownloadName)%>
                </td>
                <td>
                    <%=Html.Encode(don.Date.ToString("d-M-yyyy"))%>
                </td>
                <td>
                    <a id='lName' href='<%= String.Format("/downloads.axd?id={0}", don.DownloadId)%>' ><%= don.FileName%></a>
                 </td>
                 <td>
                    <%=Html.ActionLink(Resources.Resource.edit, "editdownloadview", 
                                                       new { downloadid = don.DownloadId,
                                                             controller = "KeyDownload",
                                                             action = "editdownloadview"
                                                       })%> <label>|</label> <a id="deleteItem" href="javascript: void(0);"><%=Resources.Resource.delete%></a>
                                                
                    <input type="hidden" id="docId" value="<%= don.DownloadId %>"/>
                </td>
               </tr>
             <%} %>
      
            </tbody>
  </table>
     <%} %>
     <input type="submit" name="CreateDownload" value="NIEWE DOWNLOAD" onclick="$('#EditForm').submit()" id="CreateDownload" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />

        </div></div>
       <div class='formBoxFooter'></div>
       </div>
 
 <% using (Html.BeginForm("create", "KeyDownload", null, FormMethod.Post, new { id = "EditForm" })) { } %>       
<div id="popup" class="popupbg" title="BEVESTIG" style="">
	 <table>
        <tr>
            <td align="center" style="text-align: center; padding: 15px 15px 15px 15px;">
                <%=Resources.Resource.deleteConfirmation%>
            </td>
        </tr>
        
    </table>
</div>

<script language="javascript" type="text/javascript">

    $(document).ready(function() {

        $("#popup").dialog(
                {
                    autoOpen: false,
                    bgiframe: false,
                    resizable: false,
                    draggable: false,
                    modal: true,
                    overlay: {
                        opacity: 0.5,
                        backgroundColor: '#fff'

                    }, buttons: {
                    '<%=Resources.Resource.delete.ToUpper()%>': function() {
                            $(this).dialog('close');
                            var id = $(this).data("id");
                            var url = "/KeyUser/KeyDownload/DeleteLinkHandler/" + id;
                            $.getJSON(url, null, function(data1) {

                            });
                            var par = $("#popup").data("parRow");
                            par.remove();
                        },
                        '<%=Resources.Resource.cancel.ToUpper()%>': function() {
                            $(this).dialog('close');
                        }
                    }
                });
        $('a#deleteItem').click(function(event) {
            event.preventDefault();
            var t2 = $(this).parent().find("input").val();
            var parentrow = $(this).parent().parent();
            $("#popup").data("id", t2);
            $("#popup").data("parRow", parentrow);
            $("#popup").dialog('open');


            return (false);

        });
    });
 </script>
</asp:Content>
