<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.Download>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.DownloadsHeader %></div></div>
        <div class='formBoxBody'>
        <div class='formBoxBodyInner'>
  <% using (Html.BeginForm("editdownload", "KeyDownload", new
   {
       downloadId = ViewData["downloadId"],
       controller = "KeyDownload",
       action = "editdownload"

   }, FormMethod.Post, new { enctype = "multipart/form-data" ,id="form1"}))
     {%>   
     <div class="error-container">
            <ul>
            </ul>
            </div>   
    <table>
    
    <tr>
        <td>
            <label id='lName'><b><%=Resources.Resource.name %></b></label>
        </td>
        <td>
         
            <%=Html.TextBox("Name", Model.Name, new { @class = "textbox" })%><%= Html.ValidationMessage("Name","* Required")%>
            <%=Html.Hidden( "downloadId",Model.DownloadId)%>
            
        </td>
    </tr>
    <tr>
        <td>
            <label id='Label1' ><b><%=Resources.Resource.description %></b></label>
        </td>
        <td>
           
            <%=Html.TextArea("Description", Model.Description, new { @class = "textarea", style = "width:200px", rows = "6", cols = "20" })%>
            <%= Html.ValidationMessage("Description", "* Required")%>
        </td>
    </tr>
    <tr>
        <td>
            <label id='Label3'><b><%=Resources.Resource.file %></b></label> 
        </td>
        
        <td>
            <input type="file" name='fuDownloadFile' id='fuDownloadFile' />
        </td>
     </tr>
</table> 
    <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
     <button name="cancelBtn" value="<%=Resources.Resource.cancel.ToUpper()%>" id="cancelBtn" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
   
    <%} %>  
  <% using (Html.BeginForm("canceldownload", "KeyDownload", new
   {
       controller = "KeyDownload",
       action = "canceldownload"

   }, FormMethod.Post, new { enctype = "multipart/form-data", id = "form2" }))
     {%>
     
     <% }%>
 
           
</div>
</div>
<div class='formBoxFooter'></div>
</div>
<script type="text/javascript">

     (function($) {
         $(document).ready(function() {
             $("#SaveButton").click(function() {
                 validateform();
             });

         });
     })(jQuery);

     function validateform() {
         $("#form1").validate({
             errorLabelContainer: $("ul", $('div.error-container')),
             wrapper: 'li',

             rules: {
                 downloadName: {
                     required: true

                 },
                 fuDownloadFile: {
                     required: false

                 }
             },
             messages: {

                 downloadName: {
                 required: "<%=Resources.Resource.nameNotCompleted%>"
                 },
                 fuDownloadFile: {
                 required: "<%=Resources.Resource.selectFileToUpload%>"
                 }
             }


         });



     }


     (function($) {
         $(document).ready(function() {
             $("#cancelBtn").click(function() {
                 //        $("#form2").submit();
                 $.post("/admindownload/canceldownload");
             });
         });


     })(jQuery);
</script>
</asp:Content>
