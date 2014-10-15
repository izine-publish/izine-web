<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<iZINE.Web.MVC.Models.Status.Edit>" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
</head>
<body>
    <div id="abusebox" style="width: 520px; height: 420px; padding: 0px 20px;">
        <div id="contentcenter" class="column">
            <form class="contentform" id="form" method="post" accept-charset="utf-8">
                <fieldset>
                      <div>
                        <label for="name">Naam</label>
                        <%= Html.TextBox("name", Model.Asset.Name, new { @class = "input", style = "width: 300px;" })%>
                        <%= Html.ValidationMessage("name")%>   
                      </div>
                      <div>
                        <label for="title">Titel</label>
                    	<%= Html.DropDownList("title", Model.Titles, Resources.Resource.selectTitle, new { style= "width: 250px;" })%>
					    <%= Html.ValidationMessage("title")%>   
                      </div>
                      <div>
                        <label for="shelve">shelve</label>
                    	<% if (Model.Shelves.Count() > 0)
                        { %>
						<%= Html.DropDownList("shelve", Model.Shelves, null, new { style = "width: 250px;" })%>
						<% }
                         else
                         { %>
						<%= Html.DropDownList("shelve", Model.Shelves, "Geen edities", new { disabled="disabled", style = "width: 250px;" })%>
						<% } %>
						<%= Html.ValidationMessage("shelve")%>   
                      </div>
                      <div>
                        <label for="status">status</label>
                    	<% if (Model.Status.Count() > 0)
                        { %>
						<%= Html.DropDownList("status", Model.Status, null, new { style = "width: 250px;" })%>
						<% }
                         else
                         { %>
						<%= Html.DropDownList("status", Model.Status, "Geen status", new { disabled = "disabled", style = "width: 250px;" })%>
						<% } %>
						<%= Html.ValidationMessage("status")%>   
                      </div>
                </fieldset>
                <fieldset class="btns">
                  <input class="submit" type="button" id="cancel" value="Annuleren" />
                  <input class="submit" type="submit" value="Opslaan" />
                </fieldset>
                <fieldset>
                    <p>
                        <label>Aangemaakt door</label>
                        Marieke Rooswinkel
                    </p>
                    <p>
                        <label>Aangemaakt op</label>
                        10-01-2010
                    </p>
                    <p>
                        <label>Laatst gewijzigd door</label>
                         Linda van Schie
                    </p>
                    <p>
                        <label>Laatst gewijzigd op</label>
                        14-01-2010
                    </p>
                    <p>
                        <label>Status</label>
                        Definitief
                    </p>
                    <p>
                        <label>Pagina</label>
                        3
                    </p>
                    <p>
                        <label>Versie</label>
                        #7
                    </p>
                    <p>
                        <label>Grootte</label>
                        6,08MB
                    </p>
                </fieldset>
            </form>
        </div>
    </div>
<script type="text/javascript">
    $(document).ready(function() {
        $("#form").validate({
            errorClass: 'validation-error',
            rules: {
                name: {
                    required: true
                }
            },
            messages: {
                name: {
                    required: ""
                }
            },
            highlight: function(element, errorClass) {
                $(element).addClass(errorClass);
                $(element.form).find("label[for=" + element.id + " .field-validation-error]").fadeIn();
            },
            unhighlight: function(element, errorClass) {
                $(element).removeClass(errorClass);
                $(element.form).find("label[for=" + element.id + " .field-validation-error]").fadeOut();
            },
            submitHandler: function() {
                jQuery.ajax({
                    type: "POST",
                    url: "/status/edit/",
                    beforeSend: function() { $.loading.showActivity(); },
                    complete: function() { $.loading.hideActivity(); },
                    dataType: "json",
                    data: {
                        assetid: '<%= Model.Asset.AssetId %>',
                        name: $('#form input[name="name"]').val()
                    },
                    success: function(data) {
                        $.fancybox.close();
                    refresh(function() {
                        });
                    },
                    error: function(data) {
                        alert('Er is een fout opgetreden tijdens het verwerken. Probeer het nogmaals.');
                    }
                });
            }
        });
    });

    $("#cancel").click(function() {
        $.fancybox.close();
    });
    
    (function($) {
        $("#title").change(function() {
            jQuery.ajax({
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
                            options.removeAttr('disabled');
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
    })(jQuery);
</script>
</body>
</html>
