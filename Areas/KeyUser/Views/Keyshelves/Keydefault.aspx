<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.KeyUser.Models.SheleveModel>>" %>
<%@ Import Namespace="MvcPaging"%>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script type="text/javascript" src="../../../../Scripts/js/jquery-ui-1.7.2.custom/jquery.pager.js"></script>
 <div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.editionHeader %></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
 
          <% using (Html.BeginForm())
             {%> 
             <table>
             <tr>
             <td>
             <label>&nbsp <%=Resources.Resource.organisation.ToLower() %>:</label>
             </td>
             <td>
             <%= Html.DropDownList("Organisation")%>
             </td>
             <td>
             <label>&nbsp;&nbsp;&nbsp; <%=Resources.Resource.title.ToLower() %>:</label>
             </td>
             <td><%= Html.DropDownList("titel",  "["+Resources.Resource.selectTitle +"]")%></td>
             <td><button name="button" type="button" onclick="$('#EditForm').submit()" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';"><%=Resources.Resource.newBtn %></button></td>
             </tr>
             </table>
               
          <%} %>
          <div id="data">
           
          <%if (Model != null && Model.Count > 0) 
           {%> 
       <table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">
            <thead>
            <tr style="background-color: #ffffff;">
                <th align="left">                                                
                    <%=Resources.Resource.edition.ToUpper() %>
                </th>
                               
                <th align="right">                                                
                    
                </th>
                
            </tr>
            
            </thead>
            <tbody>
      
            <% foreach (iZINE.Web.MVC.Areas.KeyUser.Models.SheleveModel soi in Model)
            { %>
            <tr>
                <td>
                <%= Html.Encode(soi.Name)%>
                </td>
                <td align="right">
                <%=Html.ActionLink(Resources.Resource.edit, "editshelveview", new { sheleveid = soi.SheleveId,
                                                           controller = "Keyshelves",action = "editshelveview" })%> <label>|</label> <a id="deleteItem" href="javascript: void(0);"><%=Resources.Resource.delete%></a>
                <input type="hidden" id="docId" value="<%= soi.SheleveId %>"/>
                </td>
           </tr>
           <% } %>
             
       </tbody>
                                                            
  </table>
<input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>

	

<% }%> 
	</div>
<div id="pager">
 </div>
      </div>
      <div class="formBoxFooter"></div>
      </div>
</div>
<% using (Html.BeginForm(Html.BeginForm("create", "Keyshelves", new
           {
               organisation = ViewData["OrgSelected"],
               titel = ViewData["tSelected"]
           }, FormMethod.Post, new { id = "EditForm" }))) { } %>
       
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

    (function($) {
        $(document).ready(function() {
            $('#form1').submit(function() {
                var sel = $('#Organisation option:selected').val();
                if (sel == '' || sel == undefined || sel == null) return false;

                var sel2 = $('#titel option:selected').val();
                if (sel2 == '' || sel2 == undefined || sel2 == null) return false;
            });

            $("#titel").change(function(event) {
                var sel = $('#Organisation option:selected').val();

                var sel2 = $('#titel option:selected').val();
                var callback = function(data, textStatus) {
                    $("#data").html('');
                    $("#pager").html('');
                    if (data.count == 0) return false;
                    var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
                                        '<thead>' +
                                        '<tr style="background-color: #ffffff;">' +
                                            '<th align="left">' +
                                                'EDITIES' +
                                            '</th>' +

                                            '<th align="right">' +

                                            '</th>' +

                                        '</tr>' +

                                        '</thead>');

                    var tbdy = $('<tbody>');
                    $.each(data.items, function(i, val) {
                        /////////////////////////////////////////////////////////////////////
                        var vtr = $('<tr>');
                        $('<td>').text(val.name).appendTo(vtr);

                        var vtd = $('<td align="right">');



                        $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/KeyUser/Keyshelves/editshelveview?sheleveId=' + val.id }).appendTo(vtd);
                        $(vtd).append(" | ");
                        $("<a>").text("<%=Resources.Resource.delete%>").attr({ shelveid: val.id, title: "", href: "javascript: void(0);" }).click(function(event) {
                            event.preventDefault(); var target = event.target; var shid = $(target).attr("shelveid"); var parentrow = $(this).parent().parent();
                            $("#popup").data("id", shid);
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
                $.post("/KeyUser/Keyshelves/getshelvelist", { Organisation: sel, titel: sel2, page: 1 }, callback, "json");


            });
            PageClick = function(pageclickednumber) {

                var sel = $('#Organisation option:selected').val();

                var sel2 = $('#titel option:selected').val();
                var callback = function(data, textStatus) {
                    $("#data").html('');
                    $("#pager").html('');
                    if (data.count == 0) return false;
                    var thtml = $('<table  width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">' +
                                        '<thead>' +
                                        '<tr style="background-color: #ffffff;">' +
                                            '<th align="left">' +
                                                '<%=Resources.Resource.edition.ToUpper() %>' +
                                            '</th>' +

                                            '<th align="right">' +

                                            '</th>' +

                                        '</tr>' +

                                        '</thead>');

                    var tbdy = $('<tbody>');
                    $.each(data.items, function(i, val) {
                        /////////////////////////////////////////////////////////////////////
                        var vtr = $('<tr>');
                        $('<td>').text(val.name).appendTo(vtr);

                        var vtd = $('<td align="right">');



                        $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/KeyUser/Keyshelves/editshelveview?sheleveId=' + val.id }).appendTo(vtd);
                        $(vtd).append(" | ");
                        $("<a>").text("<%=Resources.Resource.delete%>").attr({ shelveid: val.id, title: "", href: "javascript: void(0);" }).click(function(event) {
                            event.preventDefault(); var target = event.target; var shid = $(target).attr("shelveid"); var parentrow = $(this).parent().parent();
                            $("#popup").data("id", shid);
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
                $.post("/KeyUser/Keyshelves/getshelvelist", { Organisation: sel, titel: sel2, page: pageclickednumber }, callback, "json");


            }

        });
    })(jQuery);

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
                        '<%=Resources.Resource.cancel.ToUpper()%>': function() {
                            $(this).dialog('close');
                            var id = $(this).data("id");
                            var url = "/KeyUser/Keyshelves/DeleteLinkHandler/" + id;
                            $.getJSON(url, null, function(data) {

                            });
                            var par = $(this).data("parRow");
                            par.remove();
                        },
                        '<%=Resources.Resource.delete.ToUpper()%>': function() {
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

        var selectitems = document.getElementById("titel");

        var items = selectitems.getElementsByTagName("option");
        var pLenth = items.length;
        if (pLenth > 1) {
            if ($("#titel").attr("disabled") == true) { $("#titel").attr("disabled", false); }

        }
        else {
            $("#Organisation option:first-child").attr("selected", "selected");
            if ($("#titel").attr("disabled") == false) { $("#titel").attr("disabled", "disabled"); }
        }
        $("#Organisation").change(function() {
            var id = "";
            $("#Organisation option:selected").each(function() {
                id += $(this)[0].value;
            });
            if (id != '') {
                var url = "/KeyUser/Keyshelves/GetShelves/" + id;
                $.getJSON(url, null, function(data) {
                    $("#titel").empty();
                    $("#titel").append("<option value=''"
                    + "'>[<%=Resources.Resource.selectTitle%>]"
                    + "</option>");
                    var number = data.values.length;
                    if (number > 0) {
                        if ($("#titel").attr("disabled") == true) { $("#titel").attr("disabled", false); }

                    }
                    else {
                        if ($("#titel").attr("disabled") == false) { $("#titel").attr("disabled", "disabled"); }
                    }
                    $.each(data.values, function(i, values) {
                        $("#titel").append("<option value='"
                    + values.titleId
                    + "'>" + values.name
                    + "</option>");
                    });
                });
            }
            else {
                $("#data").html('');
                $("#titel").empty();
                $("#titel").append("<option value=''"
                    + "'>[<%=Resources.Resource.selectTitle%>]"
                    + "</option>");
                if ($("#titel").attr("disabled") == false) { $("#titel").attr("disabled", "disabled"); }
            }
        });
    });

    $(document).ready(function() {
        var itemcount = $("#itemcount").val();
        if (itemcount != undefined) {
            $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 5)), buttonClickCallback: PageClick });
        }
    });
   
</script>
</asp:Content>
