<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master"
	Inherits="System.Web.Mvc.ViewPage<MvcPaging.PagedList<iZINE.Web.MVC.Areas.KeyUser.Models.UnLockModel>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div class='formBox'>
		<div class='formBoxHeader'>
			<div class='formBoxHeaderLabel'>
				<%=Resources.Resource.unlockHeader %></div>
		</div>
		<div class='formBoxBody'>
			<div class='formBoxBodyInner'>
				<div id="data">
					<%if (Model != null && Model.Count > 0)
	   { %>
					<table width="810px" cellpadding="2" cellspacing="0" class="bATR_Hdr_P">
						<thead>
							<tr style="background-color: #ffffff;">
								<th align="left">
									<%=Resources.Resource.assetname %>
								</th>
								<th align="left">
									<%=Resources.Resource.appname%>
								</th>
								<th align="left">
									<%=Resources.Resource.title%>
								</th>
								<th align="left">
									<%=Resources.Resource.userName%>
								</th>
								<th align="left">
									<%=Resources.Resource.documentname%>
								</th>
								<th align="left">
									<%=Resources.Resource.date%>
								</th>
								<th align="right">
								</th>
							</tr>
						</thead>
						<tbody>
							<%foreach (iZINE.Web.MVC.Areas.KeyUser.Models.UnLockModel unlock in Model)
		 { %>
							<tr>
								<td>
									<%= Html.Encode(unlock.assetName)%>
								</td>
								<td>
									<%= Html.Encode(unlock.applicationName)%>
								</td>
								<td>
									<%= Html.Encode(unlock.titleName)%>
								</td>
								<td>
									<%= Html.Encode(unlock.userName)%>
								</td>
								<td>
									<%= Html.Encode(unlock.docName)%>
								</td>
								<td>
									<%= Html.Encode(unlock.date.ToShortDateString())%>
								</td>
								<td align="right">
									<a id="deleteItem" href="javascript: void(0);">
										<%=Resources.Resource.unlockasset%></a>
									<input type="hidden" id="assetid" value="<%= unlock.assetId %>" />
								</td>
							</tr>
							<%} %>
						</tbody>
					</table>
					<input type="hidden" id="itemcount" value='<%=ViewData.Model.TotalItemCount %>' />
					<%} %>
					<div id="pager">
					</div>
				</div>
			</div>
			<div class="formBoxFooter">
			</div>
		</div>
		<%using (Html.BeginForm()) { } %>
	</div>
	<div id="popup" class="popupbg" title="BEVESTIG">
		<table width="100%" visible="false">
			<tr>
				<td align="center" style="text-align: center; padding: 15px 15px 15px 15px;">
					<%=Resources.Resource.unlockconfirmation%>
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
                			var url = "/AdminUser/lock/DeleteLinkHandler/" + id;
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
			var itemcount = $("#itemcount").val();
			if (itemcount != undefined) {
				$("#pager").pager({ pagenumber: 1, pagecount: Math.ceil((itemcount / 15)), buttonClickCallback: PageClick });
			}
		});
		
	</script>

</asp:Content>
