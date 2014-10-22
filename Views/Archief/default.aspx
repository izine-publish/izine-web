<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Archief/default.aspx $

//    Owner: Prakash Bhatt

//    $Date: 2010-02-25 16:30:04 +0530 (Thu, 25 Feb 2010) $

//    $Revision: 860 $

//    $Author: prakash.bhatt $

//    Description: View for Archief

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Models.Result>>" %>
<%@ Register Namespace="iZINE.Web.Controls" TagPrefix="izine" %>
<%@ Import Namespace="MvcPaging"%>


    
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 <script type="text/javascript" src="/Scripts/js/jquery-ui-1.7.2.custom/js/date.format.js"></script>
<script type="text/javascript" src="http://dev.jquery.com/view/trunk/plugins/validate/jquery.validate.js"></script>
  
<style type="text/css">
	.errorIcon { color: red; }  
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.arhieveHeader %></div>
        </div>
    
        <div class='formBoxBody'">
        
            <div class='formBoxBodyInner'> 
            
            <form method="post" action="/archief/getresults" id="form1">
                 <table cellpadding="10" cellspacing="10" >
                   <tr>
                   <td> <div class="error-container">
                        <ul>
                        </ul>
                        </div>
                        </td>
                   </tr>
  
                    <tr>
                        <td ><%=Resources.Resource.search %></td>
                         <td>
                         <%= Html.ValidationMessage("query")%>
                            <%=Html.TextBox("query", ViewData["query"], new { @class = "textbox",style="width:300px"})%>
                            
                          </td>
                    </tr>
                    <tr>
                        <td ><%=Resources.Resource.author%></td>
                        <td><input type="text" id="AuthorTextBox" name="author" class="textbox" value='<%=ViewData["author"]%>' style="width:300px"/></td>
                    </tr>
                    <tr>
                        <td ><%=Resources.Resource.publication%></td>
                        <td><select id="title" name="title" class="dropdown" style="width:300px"><option selected="selected">Alle</option></select></td>
                    </tr>
                    <tr>
                        <td ><%=Resources.Resource.date%></td>
                        <td>
                            <table border="0">
                                <tr>
                                    <td><%= Html.CheckBox("dateChecked", ViewData["dateChecked"])%></td>
                                    <td>&nbsp;<%=Resources.Resource.from%></td>
                                    <td><input type="text" id="FromTextBox" name="fromDate"  class="textbox"  disabled="disabled" value='<%=ViewData["fromDate"] %>' style="width:100px"/></td>
                                    <td>&nbsp;<%=Resources.Resource.to%></td>
                                    <td>
                                        <input type="text" id="TillTextBox" name="tillDate"  class="textbox" disabled="disabled" value='<%=ViewData["tillDate"] %>' style="width:100px"/>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 200px;" class="FieldLabel">&nbsp;</td>
                        <td>
                            <input type="submit" name="Button1" value="" id="Button1" class="searchButton" onmouseover="this.className='searchButtonOver';" onmouseout="this.className='searchButton';" />
                            &nbsp;
                            <input type="reset" name="Button2" value="" id="Button2" class="resetButton" onmouseover="this.className='resetButtonOver';" onmouseout="this.className='resetButton';" />
                        </td>
                    </tr>
                    
                </table>
             </form>
              <div id="data">
               <%if (Model != null && Model.Count > 0) 
               {%>   
                    <bATR:bATRImage ID="BATRImage2" Style="h1" runat="server" Text="Resultaat" Theme="default" />              
                    <table width="810px" cellpadding="2" cellspacing="2" class="hilites">
                        <thead>
                            <tr style="background-color: #ffffff;">
                                <th align="left"><%=Resources.Resource.subject%></th>
                                <th align="left" width="100px"><%=Resources.Resource.publication%></th>
                                <th align="left"> <%=Resources.Resource.author%></th>
                                <th align="left"><%=Resources.Resource.date%></th>
                            </tr>
                
                        </thead>
                        <tbody>
                             <% foreach (iZINE.Web.MVC.Models.Result soi in Model)
                             {%>
                                <tr>
                                    <td><a id="createItem" href="javascript: void(0);" ><%=Html.Encode(soi.Subject)%></a></td>
                                    <td><%=Html.Encode(soi.Title) %></td>
                                    <td><%=Html.Encode(soi.Author) %></td>
                                    <td><%=Html.Encode(soi.PublicationDate.ToString("d-M-yyyy"))%></td>
                                 </tr>
                            <%} %>
                        </tbody>
                    </table>
                    <input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>'/>
                    <%} %>
                    </div>
                    
                    <input type="hidden" id="HiddenQuery" value='<%=ViewData["query"] %>'/>
                    <input type="hidden" id="HiddenAuthur" value='<%=ViewData["author"] %>'/>
                     <input type="hidden" id="HiddenDateChk" value='<%=ViewData["dateChecked"] %>'/>
                    <input type="hidden" id="HiddenFromDt" value='<%=ViewData["fromDate"] %>'/>
                    <input type="hidden" id="HiddenTillDt" value='<%=ViewData["tillDate"] %>'/>
                    
                    
               
                
        
        <div id="pager">
        </div>
        </div>
        </div>
       <div class="formBoxFooter"></div>
     



	
 <div id="popup" visible="false" class="popupbg" style="">
	<table align="center" class="popupdiv2" width="811">
        <tr>
            <td colspan="2">
                <asp:Panel ID="Panel1" runat="server" Height="400px">
                    <asp:Literal runat="server" ID="DocumentLiteral"></asp:Literal>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="">
                <input type="button"  id="CloseButton" value="Sluiten" />
            </td>
        </tr>
    </table>
</div>

 <script type="text/javascript">
     (function($) {
         $(document).ready(function() {
             $.validator.addMethod(
    "DateFormat",
    function(value, element) {
        return value.match(/^\d\d?\-\d\d?\-\d\d\d\d$/);

    },
    "<%=Resources.Resource.dateFormatInfo%>"
);
         });
     })(jQuery);


     (function($) {
         $(document).ready(function() {
             $("#form1").validate({
                 errorLabelContainer: $("ul", $('div.error-container')),
                 wrapper: 'li',

                 rules: {
                     query: {
                         required: true

                     },
                     fromDate: {
                         DateFormat: true,
                         required: true

                     },
                     tillDate: {
                         DateFormat: true,
                         required: true
                     }
                 },
                 messages: {

                     query: {
                         required: "Please enter zoken op"
                     },
                     fromDate: {
                     required: "<%=Resources.Resource.fromDaterequired%>"
                     },
                     tillDate: {
                     required: "<%=Resources.Resource.toDaterequired%>"
                     }
                 }


             });
         });
     })(jQuery);

     (function($) {
         $(document).ready(function() {
             $("#popup").dialog({
                 autoOpen: false,
                 bgiframe: false,
                 resizable: false,
                 draggable: false,
                 modal: true,
                 overlay: {
                     opacity: 0.5,
                     backgroundColor: '#fff'

                 },
                 open: function(event, ui) {
                 }
             });
         });

     })(jQuery);
     (function($) {
         $(document).ready(function() {
         $('a#createItem').click(function(event) {
                event.preventDefault();
                 $("#popup").dialog('open');
                 return (false);
             });
         });

     })(jQuery);
     (function($) {
         $(document).ready(function() {
             $('#CloseButton').click(function() {
                 $("#popup").dialog('close');
                 return (false);
             });
         });
     })(jQuery);
     (function($) {
         $(function() {

             var satisfied = $('#dateChecked:checked').val();
             if (satisfied != undefined) {
                 $('#FromTextBox').attr('disabled', '');
                 $('#TillTextBox').attr('disabled', '');

             }
             else {
                 $('#FromTextBox').attr('disabled', 'disabled');
                 $('#TillTextBox').attr('disabled', 'disabled');
             }

             $('#dateChecked').click(function() {
                 var satisfied = $('#dateChecked:checked').val();
                 if (satisfied != undefined) {
                     $('#FromTextBox').attr('disabled', '');
                     $('#TillTextBox').attr('disabled', '');

                 }
                 else {
                     $('#FromTextBox').attr('disabled', 'disabled');
                     $('#TillTextBox').attr('disabled', 'disabled');
                 }
             });
         });
     })(jQuery);
     (function($) {
         $(document).ready(function() {
             //for table row
             $("table.hilites tr:odd").css("background-color", "#EFF1F1");
         });
     })(jQuery);
     (function($) {
         $(document).ready(function() {



             PageClick = function(pageclickednumber) {

                 var callback = function(data, textStatus) {
                     $("#data").html('');
                     $("#pager").html('');
                     if (data.count == 0) return false;
                     var thtml = $('<table width="810px" cellpadding="2" cellspacing="2" class="hilites">' +
                        '<thead>' +
                            '<tr style="background-color: #ffffff;">' +
                                '<th align="left"><%=Resources.Resource.subject%></th>' +
                                '<th align="left" width="100px"><%=Resources.Resource.publication%></th>' +
                                '<th align="left"> <%=Resources.Resource.author%></th>' +
                                '<th align="left"><%=Resources.Resource.date%></th>' +
                            '</tr>' +

                        '</thead>');

                     var tbdy = $('<tbody>');
                     $.each(data.items, function(i, val) {
                         /////////////////////////////////////////////////////////////////////
                         var vtr = $('<tr>');
                         var vtd = $('<td>');
                         var sub = val.Subject;
                         $("<a>").text(sub).attr({ id: "createItem", title: "", href: "javascript: void(0);" }).click(function(event) {
                             event.preventDefault(); $("#popup").dialog('open');
                             return (false);
                         }).appendTo(vtd);
                         $(vtd).appendTo(vtr);

                         $('<td>').text(val.Title).appendTo(vtr);
                         $('<td>').text(val.Author).appendTo(vtr);
                         

                         $('<td>').text(val.PublicationDateString).appendTo(vtr);

                         $(vtr).appendTo(tbdy);

                         ///////////////////////////////////////////////////////////////////
                     });
                     $(tbdy).appendTo(thtml);
                     $('<bATR:bATRImage ID="BATRImage3" Style="h1" runat="server" Text="Resultaat" Theme="default" />').appendTo("#data");
                     $(thtml).appendTo("#data");



                     $("#pager").pager({ pagenumber: pageclickednumber, pagecount: Math.ceil((itemcount / 5) ), buttonClickCallback: PageClick });

                 }
                 var query = $("#HiddenQuery").val();
                 var authur = $("#HiddenAuthur").val();
                 var datechk = $("#HiddenDateChk").val();
                 var fromdate = $("#HiddenFromDt").val();
                 var tilldate = $("#HiddenTillDt").val();
                 
                 $.post("/archief/getjsonresults", { query: query, author: authur, title: null, dateChecked: datechk, fromDate: fromdate, tillDate: tilldate, page: pageclickednumber }, callback, "json");


             }
             var itemcount = $("#itemcount").val();
             if (itemcount != undefined) {
                 $("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 5) ), buttonClickCallback: PageClick });
             }
         });
     })(jQuery);     
</script>
<script type="text/jscript">

</script>
</asp:Content>

