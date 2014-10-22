<%--//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/shelves/EditShelve.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-25 16:30:04 +0530 (Thu, 25 Feb 2010) $

//    $Revision: 860 $

//    $Author: prakash.bhatt $

//    Description: view for edit shelves

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/AdminSite.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.Shelve>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
<div class='formBox'>
    <div class='formBoxHeader'>
        <div class='formBoxHeaderLabel'><%=Resources.Resource.editionHeader %></div>
     </div>
     <div class='formBoxBody'>
        <div class='formBoxBodyInner'> 
        </div>
         
   <% using (Html.BeginForm("editshelve", "shelves", new
   {
       controller = "shelves",
       action = "editshelve"

   }, FormMethod.Post, new { id = "form1" }))
      {%>        
         <div id="error-summury" >
         
         </div>      
        <table width="100%" cellpadding="5" cellspacing="10" style="">
        <tr>
            
             <td style="padding: 0px 0px 0px 25px;"><%=Resources.Resource.organisation.ToLower() %></td>
            <td ><%= Html.DropDownList("Organisation", null, "["+Resources.Resource.selectOrganisation+"]", new { style = "width:204px" })%></td>
            
            
        </tr>
        <tr>
        <td style="padding: 0px 0px 0px 25px;"><%=Resources.Resource.title.ToLower() %></td>
            <td><%= Html.DropDownList("titel", null, "["+Resources.Resource.selectTitle +"]", new { @class = "dropdownlist", @id = "titel", style = "width:204px" })%></td>
            
            
        </tr>
        <tr>
            
            <td style="padding: 0px 0px 0px 25px;"><%=Resources.Resource.name %><label id="orgError" style="color:Red;text-align:left"></label></td>
            <td>
                <input type="text" id="Name" name="Name" value='<%=Model != null ? Model.Name : ""  %>' class="textbox" style="width:200px"/>
                
             </td>
        </tr>
        
        
    </table>
    &nbsp
    <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
    <button name="button" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Submit1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
               
      <%} %>      
        
      <div class="formBoxFooter"></div>
      </div>
</div>
 <script language="javascript" type="text/javascript">

     (function($) {
         $(document).ready(function() {
             $("#SaveButton").click(function() {
                 var isvalid = true;
                 $("#error-summury").html('');
                 var ul = $('<ul style="color:Red;">');
                 var sel = $('#Organisation option:selected').val();
                 if (sel == '' || sel == undefined || sel == null) {

                     $("<li>").html('<%=Resources.Resource.selectOrganisation %>').appendTo(ul);
                     isvalid = false;
                 }

                 var sel2 = $('#titel option:selected').val();
                 if (sel2 == '' || sel2 == undefined || sel2 == null) {
                     $("<li>").html('<%=Resources.Resource.selectTtitleErr %>').appendTo(ul);
                     isvalid = false;
                 }

                 var naam = $('#Name').val();
                 if (naam == '' || naam == undefined || naam == null) {
                     $("#orgError").html('*');
                     $("<li>").html('<%=Resources.Resource.nameNotCompleted %>').appendTo(ul);
                     isvalid = false;
                 }
                 if (isvalid == false) {
                     $(ul).appendTo("#error-summury")
                     return false;
                 }

                 $("#form1").submit();

             });

         });

     })(jQuery);



     $(document).ready(function() {

         var selectitems = document.getElementById("titel");

         var items = selectitems.getElementsByTagName("option");
         var pLenth = items.length;
         if (pLenth > 1) {
             if ($("#titel").attr("disabled") == true) { $("#titel").attr("disabled", false); }

         }
         else {
             if ($("#titel").attr("disabled") == false) { $("#titel").attr("disabled", "disabled"); }
         }
         $("#Organisation").change(function() {
             var id = "";
             $("#Organisation option:selected").each(function() {
                 id = $(this)[0].value;
             });

             var url = "/shelves/GetShelves/" + id;
             $.getJSON(url, null, function(data) {
                 $("#titel").empty();
                 $("#titel").append("<option value=''"
                    + "'><%=Resources.Resource.selectTitle %>"
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
         });
     });
</script>
</asp:Content>

