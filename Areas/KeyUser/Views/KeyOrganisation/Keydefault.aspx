<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.KeyUser.Models.OrganisationModel>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.organisationHeader%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
             <div id="data">
            <%if (Model != null && Model.Count > 0) 
            {%> 
                <table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">
                    <thead>
                        <tr style="background-color: #ffffff;">
                            <th align="left">                                                
                            <%=Resources.Resource.name%>
                            </th>
                            <th align="right">                                                
                            
                            </th>
                        </tr>
            
                    </thead>
                    <tbody>
      
                        <% foreach (iZINE.Web.MVC.Areas.KeyUser.Models.OrganisationModel soi in Model)
                        { %>
                            <tr>
                                 <td>
                                       <%= Html.Encode(soi.Name)%>
                                 </td>
                                    
                                  <%--<td align="right">
                                    <%=Html.ActionLink(Resources.Resource.edit, "editorganisationview", 
                                                                       new { orgname = soi.Name,orgid = soi.OrganisationId,
                                                                           page = ViewData.Model.PageNumber,
                                                                       controller = "organisation",
                                                                       action = "editorganisationview" })%> <label>|</label> <a id="deleteItem" href="javascript: void(0);"> <%=Resources.Resource.delete%></a>
                                                                
                                    <input type="hidden" id="docId" value="<%= soi.OrganisationId %>"/>
                                 </td>--%>
                            </tr>
                     <% } %>
             
            </tbody>
                                                            
        </table>
            <input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>
         <% }%>
        </div>
        <%-- <button name="button" type="button" onclick="$('#EditForm').submit()" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';"><%=Resources.Resource.newBtn %></button>--%>
             <div id="pager"></div>     
            </div> </div>
            <div class="formBoxFooter"></div>

         </div>

<% using (Html.BeginForm("editorganisationview", "organisation", new { orgname = "", page = 1 }, FormMethod.Post, new { id = "EditForm" })) { } %>
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
                            var url = "/Organisation/DeleteLinkHandler/" + id;
                            $.getJSON(url, null, function(data) {

                            });
                            var par = $(this).data("parRow");
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



                    $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/organisation/editorganisationview?orgid=' + val.OrganisationId }).appendTo(vtd);
                    $(vtd).append(" | ");
                    $("<a>").text(" <%=Resources.Resource.delete%>").attr({ orgid: val.OrganisationId, title: "", href: "javascript: void(0);" }).click(function(event) {
                        event.preventDefault(); var target = event.target; var orgid = $(target).attr("orgid"); var parentrow = $(this).parent().parent();
                        $("#popup").data("id", orgid);
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
            $.post("/organisation/JsonIndex", { page: pageclickednumber }, callback, "json");


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
