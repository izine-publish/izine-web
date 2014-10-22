<%--
//---------------------------------------------------------------------------------------------------------------
//   $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Status/index.aspx $
//   Owner: Prakash Bhatt
//   $Date: 2010-08-10 21:49:42 +0530 (Tue, 10 Aug 2010) $
//   $Revision: 1941 $
//   $Author: remco.verhoef $
//   Description: View for status
//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Web.MVC.Models.Status.Index>" %>
<%@ Register Namespace="iZINE.Web.Controls" TagPrefix="izine" %>
<%@ Import Namespace="iZINE.Web.MVC.Extensions" %>
<%@ Import Namespace="MvcPaging" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class='formBox'>
	    <div class='formBoxHeader'>
		    <div class='formBoxHeaderLabel'>
			    <%=Resources.Resource.statusHeading%>
			</div>
	    </div>
	    <div class='formBoxBody'>
		    <div class='formBoxBodyInner'>
			    <table cellpadding="0" cellspacing="0">
				    <tr>
					    <td style="width: 45px;">
						    <label>Groep</label>
					    </td>
					    <td style="width: 210px;">
						    <%= Html.DropDownList("group", Model.Titles, "Alle", new { style= "width: 200px;" })%>
					    </td>
					    <td style="width: 40px;">
						    <label>Titel</label>
					    </td>
					    <td style="width: 210px;">
						    <%= Html.DropDownList("title", Model.Titles, Resources.Resource.selectTitle, new { style= "width: 200px;" })%>
					    </td>
					    <td style="width: 45px;">
						    <label>Editie</label>
					    </td>
					    <td>
					        <% if (Model.Shelves.Count() > 0)
                            { %>
						    <%= Html.DropDownList("shelve", Model.Shelves, null, new { style = "width: 200px;" })%>
						    <% }
                             else
                             { %>
						    <%= Html.DropDownList("shelve", Model.Shelves, "Geen edities", new { disabled="disabled", style = "width: 200px;" })%>
						    <% } %>
					    </td>
					    <td><div id="indicator" style="margin-left: 10px; display: none;"><img src="/images/28-0.gif" /></div></td>
				    </tr>
			    </table>
			    <div id="data" style="margin-top: 15px;">
			        <%=Html.Partial("Content") %>
			    </div>
		    </div>
	    </div>
	    <div class='formBoxFooter'>
	    </div>
    </div>
	<script language="javascript" type="text/javascript">
             (function($) {
                 $("#group").change(function() {
                     $.cookie("groupid", $(this).val(), { path: '/', expires: 7 });
                 });

                 $("#title").change(function() {
                     $.cookie("titleid", $(this).val(), { path: '/', expires: 7 });

                     jQuery.ajax(
                    {
                        type: "POST",
                        url: "/status/shelve",
                        data: { id: $(this).val() },
                        beforeSend: function() { $.loading.showActivity(); },
                        complete: function() { $.loading.hideActivity(); },
                        dataType: "json",
                        success: function(data) {
                            var options = $("#shelve");

                            options.empty();

                            $.each(data, function(i, shelve) {
                                var option = $("<option />").val(shelve.id).text(shelve.name);
                                options.append(option);
                            });

                            if (data.length != 0) {
                                refresh();
                            } else {
                                options.append($("<option />").text('Geen edities.'));
                                options.attr('disabled', 'disabled');
                            }
                        },
                        error: function() {
                            var options = $("#shelve");
                            options.append($("<option />").text('Fout opgetreden.'));
                            options.attr('disabled', 'disabled');
                        }
                    });
                 });

                 $("#shelve").change(function() {
                     if ($(this).val() == '')
                         return;

                     $.cookie("shelveid", $(this).val(), { path: '/', expires: 7 });
                     refresh();
                 });
             })(jQuery);

             function refresh(callback) {
                 jQuery.ajax(
                {
                    type: "POST",
                    url: "/status/index",
                    data: { id: $('shelve').val() },
                    beforeSend: function() { $('#data table tbody').empty(); $('#indicator').show(); },
                    complete: function() { $('#indicator').hide(); },
                    dataType: "html",
                    success: function(data) {
                        $("#data table tbody").replaceWith($('tbody', data));
                        if ($.isFunction(callback)) {
                            callback();
                        }

                    },
                    error: function() {
                    }
                });
             }
	</script>

</asp:Content>
