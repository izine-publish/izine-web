<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Status/default.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-07-01 15:21:26 +0530 (Thu, 01 Jul 2010) $

//    $Revision: 1593 $

//    $Author: prakash.bhatt $

//    Description: View for status

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master"
	Inherits="System.Web.Mvc.ViewPage<IEnumerable<SelectListItem>>" %>

<%@ Register Namespace="iZINE.Web.Controls" TagPrefix="izine" %>
<%@ Import Namespace="iZINE.Web.MVC.Extensions" %>
<%@ Import Namespace="MvcPaging" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
#tooltip {
	position: absolute;
	z-index: 3000;
	border: 1px solid #111;
	background-color: #eee;
	padding: 5px;
	opacity: 0.85;
}
#tooltip h3, #tooltip div { margin: 0; }
</style>
	<div class='formBox'>
		<div class='formBoxHeader'>
			<div class='formBoxHeaderLabel'>
				<%=Resources.Resource.statusHeading%></div>
		</div>
		<div class='formBoxBody'>
			<div class='formBoxBodyInner'>
				<% using (Html.BeginForm())
	   {%>
				<table>
					<tr>
						<td>
							<label>
								<%=Resources.Resource.title %>:</label>
						</td>
						<td>
							<%= Html.DropDownList("titel", Model, Resources.Resource.selectTitle)%>
						</td>
						<td>
							<label>
								&nbsp; &nbsp;<%=Resources.Resource.edition %>:</label>
						</td>
						<td>
						<%= Html.DropDownList("plank", (SelectList)ViewData["Edition"], Resources.Resource.selectEdition, new { id = "plank" })%>
							
						</td>
						<td>
							&nbsp; &nbsp;<input type="button" name="RefreshButton" value="" id="RefreshButton"
								class="refreshButton" onmouseover="this.className='refreshButtonOver';" onmouseout="this.className='refreshButton';" />
						</td>
						
					</tr>
				</table>
				<% =Html.Hidden("returnUrl", Request.QueryString["ReturnUrl"] ?? FormsAuthentication.DefaultUrl)%>
				<% } %>
				<div id="data">
				</div>
				<div id="loading" style="display: none; float: none">
					<img src="/images/loading.gif" alt="Loading" />
				</div>
			</div>
		</div>
	</div>
	<div class='formBoxFooter'>
	</div>

	<script language="javascript" type="text/javascript">
             (function($) {

             $(document).ready(function() {
                 $("#loading").bind("ajaxSend", function() {
                     $(this).show();
                 }).bind("ajaxComplete", function() {
                     $(this).hide();
                 });
             });
             
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
             				id = $(this)[0].value;
             			});

             			var url = "/Status/plank/" + id;
             			$.getJSON(url, null, function(data) {
             				$("#plank").empty();
             				$("#plank").append("<option value=''"
                    + "'><%= Resources.Resource.selectEdition%>"
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
             		
             	});
             })(jQuery);

             (function($) {
                 $(document).ready(function() {
                     //for table row
                     $("table.hilites tr:odd").css("background-color", "#EFF1F1");
                     
                     DateDeserialize = function(dateStr) {
                        return eval('new' + dateStr.replace(/\//g, ' '));
                    }
                 });
             })(jQuery);
             
        (function($) {
        	$(document).ready(function() {

        		Update = function() {
        			var sel = $('#plank option:selected').val();

        			var sel2 = $('#titel option:selected').val();
        			var callback = function(data, textStatus) {
        				$("#data").html('');
        				if (data.count == 0) return false;

        				var thtml = $('<table width="810px" cellpadding="1" cellspacing="0" class="hilites">' +
                                        '<thead>' +
                                        '<tr>' +
                                            '<th align="left"><%= Resources.Resource.status%></th>' +
                                            '<th align="left" width="100px"><%= Resources.Resource.pageNumber%></th>' +
                                            '<th align="left"><%= Resources.Resource.document%></th>' +
                                            '<th align="left"><%= Resources.Resource.date%></th>' +
                                            '<th align="left"><%= Resources.Resource.versionNumber%></th>' +
                                            '<th align="left"><%= Resources.Resource.userLastEdited%></th>' +
                                            '<th align="left">PDF</th>' +
                                            '<th align="left">&nbsp;</th>' +
                                        '</tr>' +

                                    '</thead>');

        				var tbdy = $('<tbody>');


        				$.each(data.items, function(i, val) {
        					/////////////////////////////////////////////////////////////////////
        					var vtr = $('<tr style="color:#c91787;font-size:11px;font-weight:bold;font:arial,helvetica;">');

        					var vtd = $('<td>');

        					$("<img height='16'>").attr({ style: "border-style: none", alt: "test", src: val.ImageUrl }).appendTo(vtd);
        					$(vtd).appendTo(vtr);
        					//var div = '<div> <label>' + val.PageNumber + '</label><br/><label style="font-size:10px">' + val.UserFName + ' ' + val.UserMName + ' ' + val.UserLName + '</label></div>';
        					$('<td>').html(val.PageNumber).appendTo(vtr);
        					$('<td>').text(val.Name).appendTo(vtr);
        					var dt = DateDeserialize(val.Date);

        					var curr_date = dt.getDate();

        					var curr_month = dt.getMonth() + 1;

        					var curr_year = dt.getFullYear();


        					$('<td>').text(curr_date + '-' + curr_month + '-' + curr_year).appendTo(vtr);
        					$('<td>').text('#' + val.VersionNumber).appendTo(vtr);
        					$('<td>').text(val.UserFName + ' ' + val.UserMName + ' ' + val.UserLName).appendTo(vtr);
        					var vtd1 = $('<td>');
        					var anc = $('<a>').attr({ title: "", href: "/pdf.axd?id=" + val.VersionId });
        					$("<img>").attr({ src: "/App_Themes/green/images/pdf_icon.png", width: "16", height: "16", alt: "status" }).appendTo(anc);
        					$(anc).appendTo(vtd1);
        					$(vtd1).appendTo(vtr);
        					if (val.IsLocked == true) {
        						var lTd = $('<td>');
        						$("<img>").attr({ src: "/App_Themes/green/images/Lock.png", width: "16", height: "16", alt: "lock" }).tooltip({ delay: 0, showURL: false, bodyHandler: function() { return $($("<div/>").html("locked by:"+val.LockedBy)); } }).appendTo(lTd);
        						$(lTd).appendTo(vtr);
        					}
        					$(vtr).appendTo(tbdy);
        					$.each(val.AssignmentsList, function(j, asset) {

        						var vtr1 = $('<tr height="15px" style="color:#893b96;font-size:11px;font-weight:bold;font:arial,helvetica;">');

        						var vtd2 = $('<td>');

        						$("<img height='16'>").attr({ style: "border-style: none", alt: "test", src: asset.url }).appendTo(vtd2);
        						$(vtd2).appendTo(vtr1);
        						$('<td>').appendTo(vtr1);
        						$('<td style="font-size: 11px; font-style:italic;">').text(asset.Name).appendTo(vtr1);
        						var dt1;
        						if (asset.Date != null) {
        							dt1 = DateDeserialize(asset.Date);
        						}
        						else {
        							dt1 = DateDeserialize(val.Date);
        						}

        						var curr_date1 = dt1.getDate();

        						var curr_month1 = dt1.getMonth() + 1;

        						var curr_year1 = dt1.getFullYear();


        						$('<td>').text(curr_date1 + '-' + curr_month1 + '-' + curr_year1).appendTo(vtr1);
        						$('<td>').text('#' + asset.VersionNumber).appendTo(vtr1);
        						$('<td>').text(asset.UserFName + ' ' + asset.UserMName + ' ' + asset.UserLName).appendTo(vtr1);
        						$('<td>').appendTo(vtr1);
        						$(vtr1).appendTo(tbdy);

        					});
        					var hline = $('<td colspan="7">' +
                            '<div style="border-top: 1px solid rgb(190, 204, 204); margin-top: 3px; padding-bottom: 3px; height: 1px;" width="100%"> </div>' +
                            '</td>');
        					$(hline).appendTo(tbdy);


        					///////////////////////////////////////////////////////////////////
        				});
        				var tdblank = $('<td>');
        				$('<div style="border:1px">').appendTo(tdblank);

        				$(tbdy).appendTo(thtml);
        				$(thtml).appendTo("#data");

        			}
        			$.post("/status/getjsonlist", { plank: sel, titel: sel2 }, callback, "json");

        		}
        		$("#RefreshButton").click(function(event) {
        			event.preventDefault();
        			Update();
        			

        		});
        		$("#plank").change(function(event) {
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
