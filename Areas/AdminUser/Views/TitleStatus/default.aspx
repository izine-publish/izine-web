<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/AdminUser/Views/Shared/AdminUser.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel>>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.status%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
             <div id="data">
            <%if (Model != null && Model.Count > 0) 
            {%> 
                <table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">
                    <thead>
                        <tr style="background-color: #ffffff;">
                            <th align="left" style="width:30%">                                                
                            <%=Resources.Resource.name%>
                            </th>
                            <th align="center" style="text-align:center;width:40%;">                                                
                            <%=Resources.Resource.State%>
                            </th>
                            <th align="right">                                                
                            
                            </th>
                        </tr>
            
                    </thead>
                    <tbody>
      
                        <% foreach (iZINE.Web.MVC.Areas.AdminUser.Models.TitleStatusModel soi in Model)
                        { %>
                            <tr>
                                 <td >
                                       <%= Html.Encode(soi.Name)%>
                                 </td>
                                 <td align="center">
                                       <%= Html.Encode(soi.State)%>
                                 </td>
                                    
                                  <td align="right">
                                    <%=Html.ActionLink(Resources.Resource.edit, "editstatusview", 
                                                                       new { statname = soi.Name,statusId = soi.StatusId,
                                                                           page = ViewData.Model.PageNumber,
                                                                             controller = "titlestatus",
                                                                             action = "editstatusview"
                                                                       })%> <label>|</label> <a id="deleteItem" href="javascript: void(0);"> <%=Resources.Resource.delete%></a>
                                                                
                                    <input type="hidden" id="docId" value="<%= soi.StatusId %>"/>
                                 </td>
                            </tr>
                     <% } %>
            </tbody>
                                                            
        </table>
            <input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>
         <% }%>
        </div>
         <button name="button" type="button" onclick="$('#EditForm').submit()" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';"><%=Resources.Resource.newBtn %></button>
             <div id="pager"></div>     
            </div> </div>
            <div class="formBoxFooter"></div>

         </div>
 <% using (Html.BeginForm("editstatusview", "titlestatus", new { name = "", page = 1 }, FormMethod.Post, new { id = "EditForm" })) { } %>
<div id="popup" visible="false" class="popupbg" title="BEVESTIG">
	 <table width="100%" visible="false">
        
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
                            var url = "/AdminUser/titlestatus/DeleteLinkHandler/" + id;
                            $.getJSON(url, null, function(data1) {
                                var par = $("#popup").data("parRow");
                                par.remove();
                            });

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


        PageClick = function(pageclickednumber) {

            var callback = function(data, textStatus) {
                $("#data").html('');
                $("#pager").html('');
                if (data.count == 0) return false;
                var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
                    '<thead>' +
                        '<tr style="background-color: #ffffff;">' +
                            '<th align="left">NAME</th>' +
                            '<th align="right">' +
                            '</th>' +
                        '</tr>' +
                    '</thead>');


                var tbdy = $('<tbody>');
                $.each(data.items, function(i, val) {
                    /////////////////////////////////////////////////////////////////////
                    var vtr = $('<tr>');
                    $('<td>').text(val.Name).appendTo(vtr);

                    var vtd = $('<td align="right">');



                    $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/AdminUser/titlestatus/editstatusview?statusid=' + val.statusid }).appendTo(vtd);
                    $(vtd).append(" | ");
                    $("<a>").text(" <%=Resources.Resource.delete%>").attr({ statusid: val.statusid, title: "", href: "javascript: void(0);" }).click(function(event) {
                        event.preventDefault(); var target = event.target; var statusid = $(target).attr("statusid"); var parentrow = $(this).parent().parent();
                        $("#popup").data("id", statusid);
                        $("#popup").data("parRow", parentrow);
                        $("#popup").dialog('open');
                    }).appendTo(vtd);
                    $(vtd).appendTo(vtr);

                    $(vtr).appendTo(tbdy);

                    ///////////////////////////////////////////////////////////////////
                });
                $(tbdy).appendTo(thtml);
                $(thtml).appendTo("#data");

                $("#pager").pager({ pagenumber: pageclickednumber, pagecount: Math.ceil((data.count / 15)), buttonClickCallback: PageClick });

            }
            $.post("/AdminUser/titlestatus/JsonIndex", { page: pageclickednumber }, callback, "json");
        }
    });

    $(document).ready(function() {
        var itemcount = $("#itemcount").val();
        if (itemcount != undefined) {
            $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 15) ), buttonClickCallback: PageClick });
        }
    });
 </script>
</asp:Content>
