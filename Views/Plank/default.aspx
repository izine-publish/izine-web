<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Plank/default.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: view for plank

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master"
	Inherits="System.Web.Mvc.ViewPage<IEnumerable<SelectListItem>>" %>

<%@ Import Namespace="MvcPaging" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<% using (Html.BeginForm())
	{%>
	<div class='formBox'>
		<div class='formBoxHeader'>
			<div class='formBoxHeaderLabel'>
				<%=Resources.Resource.publicationHeader %></div>
		</div>
		<div class='formBoxBody'>
			<div class='formBoxBodyInner' id="formbox" >
			
				<table>
					<tr>
						<td>
							<label>
								<%=Resources.Resource.title.ToLower() %>:</label>
						</td>
						<td>
							<%= Html.DropDownList("titel", Model, Resources.Resource.selectTitle )%>
						</td>
						<td>
							<label>
								&nbsp; &nbsp;<%=Resources.Resource.edition.ToLower() %>:</label>
						</td>
						<td>
							<%= Html.DropDownList("plank", null, Resources.Resource.selectEdition, new { @class = "dropdownlist" })%>
						</td>
						<td>
							&nbsp; &nbsp;<input type="button" name="RefreshButton" value="" id="RefreshButton"
								class="refreshButton" onmouseover="this.className='refreshButtonOver';" onmouseout="this.className='refreshButton';" />
						</td>
					</tr>
				</table>
			</div>
		</div>
		<div class="formBoxFooter">
		</div>
	</div>
	<%} %>

	<script language="javascript" type="text/javascript">

		(function($) {
			$(document).ready(function() {
				$('#form1').submit(function() {
					var sel = $('#plank option:selected').val();
					if (sel == '' || sel == undefined || sel == null) return false;

					var sel2 = $('#titel option:selected').val();
					if (sel2 == '' || sel2 == undefined || sel2 == null) return false;
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

		  fillPlank = function() {
		   var id = "";
		   $("#titel option:selected").each(function() {
		    id += $(this)[0].value;
		   });

		   var url = "/Plank/plank/" + id;
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
		  }
		  $("#titel").change(function() {
		   fillPlank();
		  });
		  blankPreview = function() {
		   var thtml = '<table cellpadding="0" width="120px" cellspacing="0">' +
                        '<tbody><tr>' +
                            '<td colspan="0">' +
                                '&nbsp;' +

                            '</td>' +

                        '</tr>' +
                        '<tr>' +
                            '<td colspan="0" align="left">' +



                            '</td>' +
                       '</tr>' +
                        '<tr>' +
                            '<td colspan="0" style="padding-left: 15px;" align="right">' +

                            '</td>' +
                        '</tr>' +
                    '</tbody>' +
                    '</table>';
		   return thtml;
		  }
		  docPreview = function(data) {
		   var dochtml = '<table cellpadding="0" cellspacing="0" >' +
        '<tbody>' +
            '<tr>' +
                '<td colspan="2">' +
                    '&nbsp;' +
                    '<div id="dHeading" style="padding: 1px 0pt; overflow: hidden;text-align:left; width: 120px; height: 20px; color: Black; font-size: 9px ; padding:1px 0 1px 0;">' +
                        '<b><span id="sNumber">' + data.Number.toString() + '</span>.  <span id="sName" title="' + data.Number.toString() + '">&nbsp;&nbsp;' + data.Name + '</span></b></div>' +
                '</td>' +

            '</tr>' +
            '<tr>' +
                '<td colspan="2" align="left" style="border:1px #999 solid;">' +

                    '<a id="ThumbnailLinkButton" src="/image.axd?id=' + data.PageId + '" rel="lightbox[roadtrip]" title="' + data.Name + '"' +
                    '" style="cursor: pointer; display:inline-block; height: 175px; width: 118px;text-decoration:none;">' +
                    '<img id="PageImage" src="/image.axd?id=' + data.PageId + '" style="border:0px; height: 175px; width: 118px; text-align:right" border="0" alt="" /></a>' +
                '</td>' +
            '</tr>' +
            '<tr>' +
                '<td colspan="2" align="right">' +
                 '<div width="100px" style="padding: 2px 0 0 0">';


		   dochtml += '<img id="statusImage" src="' + data.StatusImage + '"  alt="" align="left" width="56" height="16" style="margin-right:15px"/>';
		   if (data.StatusState != null && "a40327cf-5190-4694-94ed-babaeda98c3f" == data.StatusState.toString()) {

		    dochtml += '<img id="CheckImage" src="/App_Themes/green/images/check.png" width="16" height="16" alt="" align="left"/>';
		   }
		   dochtml += '<a id="HyperLinkPDFPage" href="/pdf.axd?id=' + data.VersionId + '"><img  src="/App_Themes/green/images/pdf_icon.png" style="border-width: 0px; height: 16px; width: 16px;" alt=""/></a>';
		   dochtml += '</div></td></tr></tbody></table>';

		   return dochtml;


		  }

		  Update = function() {
		   var sel = $('#plank option:selected').val();

		   var sel2 = $('#titel option:selected').val();
		   var callback = function(data, textStatus) {
		    $("#data").html('');
		    if (data.count == 0) {
		     $("#data-table").remove();
		     return false;
		    }

		    var formdata = document.getElementById("data-table");
		    if (formdata == undefined) {
		     $('<table width="90%" id="data-table">' +
                            '<tr >' +
                                '<td colspan="3" id="data"></td>' +
                            '</tr>' +
                       '</table>').appendTo("#formbox");

		    }

		    var shtml = $('<div style="margin-left:-6px; width: 825px;">');
		    $.each(data.items, function(i, val) {
		     if (val.First.IsBlank == true && val.Second.IsBlank == true)
		     { return true; }

		     var dhtml = '<div id="PagePanel" style="width:266px;text-align:right;" class="documentPages" >' +
                            '<table style="margin-left: 2px;margin-right:2px; margin-bottom: 0px; text-align:right;width:255px"" cellspacing="8">' +
                                '<tr >' +
                                    '<td align="center">';
		     var FirstPreview = "";
		     if (val.First.IsBlank == true) {
		      FirstPreview = blankPreview();
		     }
		     else {
		      FirstPreview = docPreview(val.First);
		     }
		     dhtml += FirstPreview;
		     dhtml += '</td><td>';
		     var SecondPreview = "";
		     if (val.Second.IsBlank == true) {
		      SecondPreview = blankPreview();
		     }
		     else {
		      SecondPreview = docPreview(val.Second);
		     }
		     dhtml += SecondPreview;
		     dhtml += '</td><tr></table></div>';
		     $(dhtml).appendTo(shtml);
		    });


		    $(shtml).appendTo("#data");


		   }
		   $.post("/plank/getjsonlist", { plank: sel, titel: sel2, page: 1 }, callback, "json");



		  }
		  //-------------------------------------------------------------------------------------//
		  $("#plank").change(function(event) {
		   Update();
		  });


		 });


		 $(document).ready(function() {

		  $("a#ThumbnailLinkButton").live('click', function() {
		  $.fancybox({
		    'type': 'image',
		    'padding': 0,
		    'autoScale': false,
		    'transitionIn': 'none',
		    'transitionOut': 'none',
		    'title': $(this).attr('title'),
		    'centerOnScroll': true,
		    'href': $(this).attr('src'),
		    'cache': false,
		    onComplete: function() {
		    },
		    onCleanup: function() {
		    }
		   });
		  });

		  $("#RefreshButton").click(function(event) {
		   event.preventDefault();
		   Update();

		  });

		  var nplank = $("#plank option").length;
		  if (nplank == 1) {
		   fillPlank();
		  }
		  else {
		   Update();
		  }
		 });

		})(jQuery);
      
	</script>

</asp:Content>
