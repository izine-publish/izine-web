<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Pdf/default.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-25 16:30:04 +0530 (Thu, 25 Feb 2010) $

//    $Revision: 860 $

//    $Author: prakash.bhatt $

//    Description: View for certified Pdf

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Models.C_PdfModel>>" %>

<%@ Register Namespace="iZINE.Web.Controls" TagPrefix="izine" %>
<%@ Import Namespace="iZINE.Web.MVC.Extensions" %>
<%@ Import Namespace="MvcPaging"%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
        <div class='formBox'>
            <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.certifiedPdfHeader %></div>
        </div>
        
        <div class='formBoxBody'>
            <div class='formBoxBodyInner' id="formbox"> 
              <% using (Html.BeginForm(new { plank = ViewData["plank"], titel = ViewData["titel"], controller = "pdf", action = "getlist" }))
                {%>          
                   <table>
                    <tr>
                        <td>
                            <label>titel:</label>
                        </td>
                        <td>
                            <%= Html.DropDownList("titel", (SelectList)ViewData["titel"], Resources.Resource.selectTitle, new { @class = "dropdown" })%>
                         </td>
                        <td>
                            <label>&nbsp;&nbsp; editie:</label>
                        </td>
                        
                        <td>
                            <%= Html.DropDownList("plank", (SelectList)ViewData["plank"], Resources.Resource.selectEdition, new { @class = "dropdown" })%>
                        </td>
                        <td>
                        &nbsp;
                            <button name="button" type="button" onclick="(function($) {$('#EditForm').submit() })(jQuery);  " class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';"><%=Resources.Resource.newBtn %></button>
                    
                        </td>
                    </tr>
                </table>
               
                   <%} %> 
                    <div id="data"> 
                    <%if (Model != null && Model.Count > 0) 
                    {%> 
                        <table width="810px" cellpadding="2" cellspacing="0" style="border: 1px solid rgb(165, 189, 49); background-color: rgb(239, 242, 191);">
                        <thead>
                            <tr  style="background-color: rgb(165, 190, 49);">
                                <th align="left"></th>
                                <th ><h2><span style="color: #FFFFFF;"><%=Resources.Resource.document %></span></h2></th>
                                <th ><h2><span style="color: #FFFFFF;"><%=Resources.Resource.versionNumber %></span></h2></th>
                                <th ><h2><span style="color: #FFFFFF;"><%=Resources.Resource.date %></span></h2></th>
                                <th ><h2><span style="color: #FFFFFF;"><%=Resources.Resource.user %></span></h2></th>
                                <th align="right"></th>
                           </tr>
                        </thead>
                        <tbody>
                            <% foreach (iZINE.Web.MVC.Models.C_PdfModel soi in Model)
                            { %>
                                <tr>
                                    <td align="center">
                                        <a href="<%= Html.Encode("/asset.axd?id="+soi.VersionId)%>" >
                                            <img id="Image4"  src="/App_Themes/green/images/pdf_icon.png" width="16" height="16" alt=""/></a>
                                        </td>
                                    <td align="center"><%= Html.Encode(soi.Name)%></td>
                                    <td align="center"><%= Html.Encode("#"+soi.VersionNumber)%></td>
                                    <td align="center"><%= Html.Encode(soi.Date.Value.ToString("d-M-yyyy"))%></td>
                                    <td align="center"><%= Html.Encode(soi.UserName)%></td>
                                    <td align="center">
                                     <%=Html.ActionLink(Resources.Resource.edit, "editdocumentview", new
                                                                        {
                                                                            dname = soi.Name,
                                                                            tname = soi.titleName,shName= soi.sheleveName,
                                                                            docid = soi.DocumentId,
                                                                            sheleveid = soi.StatusId,
                                                                            titleid = soi.titileId,
                                                                            controller = "pdf",
                                                                            action = "editdocumentview" 
                                                                        })%> <label>|</label> <a id="deleteItem" href="javascript: void(0);" ><%=Resources.Resource.delete%></a>
                                                                        
                                    <input type="hidden" id="docId" value="<%= soi.DocumentId %>"/>
                                   </td>
                                </tr>
                        <% } %>
                    </tbody>
                </table>
  
                <input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>
        <% }%>  
 </div>
   
 
<% using (Html.BeginForm("create", "pdf", new { sheleveId = ViewData["pSelected"], titleId = ViewData["tSelected"] }, FormMethod.Post, new { id = "EditForm" })) { } %>

    <div id="popup" visible="false" title="BEVESTIG">
	     <table width="100%" visible="false">
             <tr>
                <td align="center" style="text-align: center; padding: 15px 15px 15px 15px;">
                    <%=Resources.Resource.deleteConfirmation%> 
                </td>
            </tr>
            
        </table>
    </div>
  
   
 </div></div>
   <div class='formBoxFooter'></div>
   </div>
   
   <script language="javascript" type="text/javascript">

       (function($) {
           $(document).ready(function() {
               $('#EditForm').submit(function() {
                   var sel = $('#plank option:selected').val();
                   if (sel == '' || sel == undefined || sel == null) return false;

                   var sel2 = $('#titel option:selected').val();
                   if (sel2 == '' || sel2 == undefined || sel2 == null) return false;
               });
           });
       })(jQuery);

       (function($) {
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
                            var url = "/pdf/DeleteLinkHandler/" + id;
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
           });
       })(jQuery);

       (function($) {
           $(document).ready(function() {

               var selectitems = document.getElementById("plank");

               var items = selectitems.getElementsByTagName("option");
               var pLenth = items.length;
               if (pLenth > 1) {
                   if ($("#plank").attr("disabled") == true) { $("#plank").attr("disabled", false); }

               }
               else {
                   if ($("#plank").attr("disabled") == false) { $("#plank").attr("disabled", "disabled"); }
               }
               $("#titel").change(function() {
                   var id = "";
                   $("#titel option:selected").each(function() {
                       id += $(this)[0].value;
                   });

                   var url = "/pdf/GetShelves/" + id;
                   $.getJSON(url, null, function(data) {
                       $("#plank").empty();
                       $("#plank").append("<option value=''"
                    + "'><%=Resources.Resource.selectEdition %>"
                    + "</option>");
                       var number = data.values.length;
                       if (number > 0) {
                           if ($("#plank").attr("disabled") == true) { $("#plank").attr("disabled", false); }

                       }
                       else {
                           if ($("#plank").attr("disabled") == false) { $("#plank").attr("disabled", "disabled"); }
                       }
                       $.each(data.values, function(i, values) {
                           $("#plank").append("<option value='"
                    + values.id
                    + "'>" + values.name
                    + "</option>");
                       });
                   });
               });
           });
       })(jQuery);

       (function($) {
           $(document).ready(function() {
               //for table row
               $("table.hilites tr:odd").css("background-color", "#EFF1F1");
               $("table.hilites tr:even").css("background-color", "#FFF2BF");

           });
       })(jQuery);
(function($) {
    $(document).ready(function() {
        $("#plank").change(function(event) {
            var sel = $('#titel option:selected').val();

            var sel2 = $('#plank option:selected').val();
            var callback = function(data, textStatus) {
                $("#data").html('');
                $("#pager").html('');
                if (data.count == 0) {
                    $("#pager").remove();
                    return false;
                }
                var pager = document.getElementById("pager");
                if (pager == undefined) {
                    $('<div id="pager">').appendTo("#formbox");
                }
                var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="hilites" style="border: 1px solid rgb(165, 189, 49); background-color: rgb(239, 242, 191);">' +
                        '<thead>' +
                            '<tr  style="background-color: rgb(165, 190, 49);">' +
                                '<th align="left" scope="col"></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.document %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.versionNumber %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.date %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.user %></span></h2></th>' +
                                '<th align="right" scope="col"></th>' +
                           '</tr>' +
                        '</thead>');

                var tbdy = $('<tbody>');
                $.each(data.items, function(i, val) {
                    /////////////////////////////////////////////////////////////////////

                    var vtr = $('<tr>');

                    var td1 = $('<td align="center">');
                    var atag = $('<a href="/asset.axd?id=' + val.VersionId + '"><img src="/App_Themes/green/images/pdf_icon.png" width="16" height="16" alt=""/></a>');
                    $(atag).appendTo(td1);
                    $(td1).appendTo(vtr);
                    $('<td align="center">').text(val.Name).appendTo(vtr);
                    $('<td align="center">').text('#' + val.VersionNumber).appendTo(vtr);
                    var dt = DateDeserialize(val.Date);

                    var curr_date = dt.getDate();

                    var curr_month = dt.getMonth() + 1;

                    var curr_year = dt.getFullYear();


                    $('<td align="center">').text(curr_date + '-' + curr_month + '-' + curr_year).appendTo(vtr);
                    $('<td align="center">').text(val.UserName).appendTo(vtr);


                    var vtd = $('<td align="center">');



                    $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/pdf/editdocumentview?docid=' + val.DocumentId }).appendTo(vtd);
                    $(vtd).append(" | ");
                    $("<a>").text("<%=Resources.Resource.delete%>").attr({ docId: val.DocumentId, title: "", href: "javascript: void(0);" }).click(function(event) {
                        event.preventDefault(); var target = event.target; var docId = $(target).attr("docId"); var parentrow = $(this).parent().parent();
                        $("#popup").data("id", docId);
                        $("#popup").data("parRow", parentrow);
                        $("#popup").dialog('open');
                    }).appendTo(vtd);
                    $(vtd).appendTo(vtr);

                    $(vtr).appendTo(tbdy);

                    ///////////////////////////////////////////////////////////////////
                });
                $(tbdy).appendTo(thtml);
                $(thtml).appendTo("#data");

                $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((data.count / 10)), buttonClickCallback: PageClick });

            }
            $.post("/pdf/getjsonlist", { plank: sel2, titel: sel, page: 1 }, callback, "json");


        });
        PageClick = function(pageclickednumber) {

            var sel = $('#titel option:selected').val();

            var sel2 = $('#plank option:selected').val();
            var callback = function(data, textStatus) {
                $("#data").html('');
                $("#pager").html('');
                if (data.count == 0) return false;
                var thtml = $('<table width="810px" cellpadding="2" cellspacing="0" class="hilites" style="border: 1px solid rgb(165, 189, 49); background-color: rgb(239, 242, 191);">' +
                        '<thead>' +
                            '<tr align="left" style="background-color: rgb(165, 190, 49);">' +
                                '<th align="left" scope="col"></th>' +
                                '<th scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.document %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.versionNumber %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.date %></span></h2></th>' +
                                '<th  scope="col"><h2><span style="color: #FFFFFF;"><%=Resources.Resource.user %></span></h2></th>' +
                                '<th  scope="col"></th>' +
                           '</tr>' +
                        '</thead>');

                var tbdy = $('<tbody>');
                $.each(data.items, function(i, val) {
                    /////////////////////////////////////////////////////////////////////
                    var vtr = $('<tr>');
                    var td1 = $('<td align="center">');
                    var atag = $('<a href="/asset.axd?id=' + val.VersionId + '"><img id="Image4"  src="/App_Themes/green/images/pdf_icon.png" width="16" height="16" alt=""/></a>');
                    $(atag).appendTo(td1);
                    $(td1).appendTo(vtr);
                    $('<td align="center">').text(val.Name).appendTo(vtr);
                    $('<td align="center">').text('#' + val.VersionNumber).appendTo(vtr);
                    var dt = DateDeserialize(val.Date);

                    var curr_date = dt.getDate();

                    var curr_month = dt.getMonth() + 1;

                    var curr_year = dt.getFullYear();


                    $('<td align="center">').text(curr_date + '-' + curr_month + '-' + curr_year).appendTo(vtr);
                    $('<td align="center">').text(val.UserName).appendTo(vtr);

                    var vtd = $('<td align="center">');



                    $("<a>").text("<%=Resources.Resource.edit%>").attr({ href: '/pdf/editdocumentview?docid=' + val.DocumentId }).appendTo(vtd);
                    $(vtd).append(" | ");
                    $("<a>").text("<%=Resources.Resource.delete%>").attr({ docId: val.DocumentId, title: "", href: "javascript: void(0);" }).click(function(event) {
                        event.preventDefault(); var target = event.target; var docId = $(target).attr("docId"); var parentrow = $(this).parent().parent();
                        $("#popup").data("id", docId);
                        $("#popup").data("parRow", parentrow);
                        $("#popup").dialog('open');
                    }).appendTo(vtd);
                    $(vtd).appendTo(vtr);

                    $(vtr).appendTo(tbdy);

                    ///////////////////////////////////////////////////////////////////
                });
                $(tbdy).appendTo(thtml);
                $(thtml).appendTo("#data");

                $("#pager").pager({ pagenumber: pageclickednumber, pagecount: Math.ceil((data.count / 10)), buttonClickCallback: PageClick });

            }
            $.post("/pdf/getjsonlist", { plank: sel2, titel: sel, page: pageclickednumber }, callback, "json");


        }


    });

    $(document).ready(function() {
        var itemcount = $("#itemcount").val();
        if (itemcount != undefined) {
            $('<div id="pager">').appendTo("#formbox");
            $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 10)), buttonClickCallback: PageClick });
        }

        DateDeserialize = function(dateStr) {
            return eval('new' + dateStr.replace(/\//g, ' '));
        }
    });
})(jQuery);


    </script>
</asp:Content>

