<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/AdminUser/Views/Shared/AdminUser.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.AdminUser.Models.UserModel>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

   <div class='formBox'>
    <div class='formBoxHeader'>
        <div class='formBoxHeaderLabel'><%=Resources.Resource.userHeader %></div>
     </div>
     <div class='formBoxBody'>
        <div class='formBoxBodyInner'> 
        
        
        <% using (Html.BeginForm(new { organisation = ViewData["Organisation"], controller = "users", action = "getlist" }))
          {%> 
          <table>
            <tr>
                <td>
                <label>&nbsp <%=Resources.Resource.organisation %>:</label> 
                </td>
                <td>
                    <%= Html.DropDownList("Organisation", null, "[" + Resources.Resource.selectOrganisation + "]", new { @class = "dropdownlist", @id = "Organisation" })%>                    
                </td>
                
               <td>
            
                <button name="button" type="button" onclick="$('#EditForm').submit()" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';"><%=Resources.Resource.newBtn.ToUpper()+" "+ Resources.Resource.user.ToUpper()%></button>
            </td>
            </tr>
            </table>
          <%} %>
          <div id="data">
  <%if (Model != null && Model.Count > 0) 
           {%> 
        <table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P" style="margin-top:10px">
            <thead>
            <tr style="background-color: #ffffff;">
                <th align="left">                                                
                    <%=Resources.Resource.name.ToUpper() %>
                </th>
                <th align="left">                                                
                     <%=Resources.Resource.userName.ToUpper() %>
                </th>
                <th align="left">                                                
                    <%=Resources.Resource.role.ToUpper() %>
                </th>
                <th align="right">                                                
                   
                </th>
            </tr>
            </thead>
            <tbody>
            <% foreach (iZINE.Web.MVC.Areas.AdminUser.Models.UserModel soi in Model)
               { %>
            <tr>
                <td>
                      <%= Html.Encode(soi.FirstName + " " +soi.MiddleName +" "+soi.LastName)%>
                </td>
                <td>
                      <%= Html.Encode(soi.UserName)%>
                </td>
                <td>
                      <%= Html.Encode(soi.RoleName)%>
                </td>
                <td>
                |<%=Html.ActionLink(Resources.Resource.edit, "edituserview", 
                                            new { userid = soi.UserId }
                                    )%> <label>|</label> <a id="deleteItem" href="javascript: void(0);"> <%=Resources.Resource.delete%></a>
                <input type="hidden" id="docId" value="<%= soi.UserId %>"/>
                </td>
            </tr>
          <% } %>
       </tbody>
  </table>
  <input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>

<% }%>  

</div>
<div id="pager"></div> 
</div>      
        
        <div class="formBoxFooter"></div>
      </div>
</div>
<% using (Html.BeginForm("create", "users", FormMethod.Post, new { id = "EditForm" })) { } %>
       
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
                            var url = "/AdminUser/users/DeleteLinkHandler/" + id;
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
        });
        $(document).ready(function() {
            $("#Organisation").change(function(event) {
                var sel = $('#Organisation option:selected').val();

                if (sel != '') {
                    var callback = function(data, textStatus) {
                        $("#data").html('');
                        $("#pager").html('');
                        if (data.count == 0) return false;
                        var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
            '<thead>' +
            '<tr style="background-color: #ffffff;">' +
                '<th align="left">' +
                    '<%=Resources.Resource.name.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    '<%=Resources.Resource.userName.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    '<%=Resources.Resource.role.ToUpper() %>' +
                '</th>' +
                '<th align="right">' +

                '</th>' +
            '</tr>' +
            '</thead>');

                        var tbdy = $('<tbody>');
                        $.each(data.items, function(i, val) {
                            /////////////////////////////////////////////////////////////////////

                            var vtr = $('<tr>');
                            $('<td>').text(val.FirstName + ' ' + val.MiddleName + ' ' + val.LastName).appendTo(vtr);
                            $('<td>').text(val.UserName == null ? "" : val.UserName).appendTo(vtr);
                            $('<td>').text(val.RoleName).appendTo(vtr);

                            var vtd = $('<td>');



                            $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/AdminUser/users/edituserview?userid=' + val.UserId }).appendTo(vtd);
                            $(vtd).append(" | ");
                            $("<a>").text(" <%=Resources.Resource.delete%>").attr({ userid: val.UserId, title: "", href: "javascript: void(0);" }).click(function(event) {
                                event.preventDefault(); var target = event.target; var userid = $(target).attr("userid"); var parentrow = $(this).parent().parent();
                                $("#popup").data("id", userid);
                                $("#popup").data("parRow", parentrow);
                                $("#popup").dialog('open');
                            }).appendTo(vtd);
                            $(vtd).appendTo(vtr);

                            $(vtr).appendTo(tbdy);

                            ///////////////////////////////////////////////////////////////////
                        });
                        $(tbdy).appendTo(thtml);
                        $(thtml).appendTo("#data");

                        $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((data.count / 5)), buttonClickCallback: PageClick });

                    }
                    $.post("/AdminUser/users/getjsonlist", { Organisation: sel, page: 1 }, callback, "json");
                }
                else {
                    $("#data").html('');
                }
            });
            PageClick = function(pageclickednumber) {

                var sel = $('#Organisation option:selected').val();

                var callback = function(data, textStatus) {
                    $("#data").html('');
                    $("#pager").html('');
                    if (data.count == 0) return false;
                    var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
            '<thead>' +
            '<tr style="background-color: #ffffff;">' +
                '<th align="left">' +
                    ' <%=Resources.Resource.name.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    ' <%=Resources.Resource.userName.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    ' <%=Resources.Resource.role.ToUpper() %>' +
                '</th>' +
                '<th align="right">' +

                '</th>' +
            '</tr>' +
            '</thead>');

                    var tbdy = $('<tbody>');
                    $.each(data.items, function(i, val) {
                        /////////////////////////////////////////////////////////////////////

                        var vtr = $('<tr>');
                        $('<td>').text(val.FirstName + ' ' + val.MiddleName + ' ' + val.LastName).appendTo(vtr);
                        $('<td>').text(val.UserName).appendTo(vtr);
                        $('<td>').text(val.RoleName).appendTo(vtr);

                        var vtd = $('<td>');



                        $("<a>").text("<%=Resources.Resource.edit %>").attr({ href: '/AdminUser/users/edituserview?userid=' + val.UserId }).appendTo(vtd);
                        $(vtd).append(" | ");
                        $("<a>").text(" <%=Resources.Resource.delete%>").attr({ userid: val.UserId, title: "", href: "javascript: void(0);" }).click(function(event) {
                            event.preventDefault(); var target = event.target; var userid = $(target).attr("userid"); var parentrow = $(this).parent().parent();
                            $("#popup").data("id", userid);
                            $("#popup").data("parRow", parentrow);
                            $("#popup").dialog('open');
                        }).appendTo(vtd);
                        $(vtd).appendTo(vtr);

                        $(vtr).appendTo(tbdy);

                        ///////////////////////////////////////////////////////////////////
                    });
                    $(tbdy).appendTo(thtml);
                    $(thtml).appendTo("#data");

                    $("#pager").pager({ pagenumber: pageclickednumber, pagecount: Math.ceil((data.count / 5)), buttonClickCallback: PageClick });

                }
                $.post("/AdminUser/users/getjsonlist", { Organisation: sel, page: pageclickednumber }, callback, "json");
            }
        });

        $(document).ready(function() {
            var itemcount = $("#itemcount").val();
            if (itemcount != undefined) {                
                $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 5)), buttonClickCallback: PageClick });
            }
            var sel = $('#Organisation option:selected').val();
            if (sel != '') {
                var callback = function(data, textStatus) {
                    $("#data").html('');
                    $("#pager").html('');
                    if (data.count == 0) return false;
                    var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
            '<thead>' +
            '<tr style="background-color: #ffffff;">' +
                '<th align="left">' +
                    '<%=Resources.Resource.name.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    '<%=Resources.Resource.userName.ToUpper() %>' +
                '</th>' +
                '<th align="left">' +
                    '<%=Resources.Resource.role.ToUpper() %>' +
                '</th>' +
                '<th align="right">' +

                '</th>' +
            '</tr>' +
            '</thead>');

                    var tbdy = $('<tbody>');
                    $.each(data.items, function(i, val) {
                        /////////////////////////////////////////////////////////////////////

                        var vtr = $('<tr>');
                        $('<td>').text(val.FirstName + ' ' + val.MiddleName + ' ' + val.LastName).appendTo(vtr);
                        $('<td>').text(val.UserName == null ? "" : val.UserName).appendTo(vtr);
                        $('<td>').text(val.RoleName).appendTo(vtr);

                        var vtd = $('<td>');



                        $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/AdminUser/users/edituserview?userid=' + val.UserId }).appendTo(vtd);
                        $(vtd).append(" | ");
                        $("<a>").text(" <%=Resources.Resource.delete%>").attr({ userid: val.UserId, title: "", href: "javascript: void(0);" }).click(function(event) {
                            event.preventDefault(); var target = event.target; var userid = $(target).attr("userid"); var parentrow = $(this).parent().parent();
                            $("#popup").data("id", userid);
                            $("#popup").data("parRow", parentrow);
                            $("#popup").dialog('open');
                        }).appendTo(vtd);
                        $(vtd).appendTo(vtr);

                        $(vtr).appendTo(tbdy);

                        ///////////////////////////////////////////////////////////////////
                    });
                    $(tbdy).appendTo(thtml);
                    $(thtml).appendTo("#data");

                    $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((data.count / 5)), buttonClickCallback: PageClick });

                }
                $.post("/AdminUser/users/getjsonlist", { Organisation: sel, page: 1 }, callback, "json");
            }

        });
       
    </script>
</asp:Content>
