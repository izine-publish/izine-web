<%--//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Pdf/Edit.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-25 16:30:04 +0530 (Thu, 25 Feb 2010) $

//    $Revision: 860 $

//    $Author: prakash.bhatt $

//    Description: View for edit certified Pdf

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.Asset>" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" src="http://dev.jquery.com/view/trunk/plugins/validate/jquery.validate.js"></script>
 <div class='formBox'>
        <div class='formBoxHeader'><div class='formBoxHeaderLabel'><%=Resources.Resource.certifiedPdfHeader %></div></div>
        <div class='formBoxBody'>
        <div class='formBoxBodyInner'>

<% using (Html.BeginForm("editdocument", "pdf",new
   {
       tname = ViewData["tName"],
       docid = ViewData["docId"],
       sheleveid = ViewData["sheleveId"],
       titleid = ViewData["titleId"],
       controller = "pdf",
       action = "editdocument"
       
   }, FormMethod.Post, new { enctype = "multipart/form-data",id="form1" }))
   {%>
   <table width="100%" cellpadding="2" cellspacing="2" style="">
        <tr>
            <td style="width: 200px;"><%=Resources.Resource.name %></td>
            <td>
            
                <input type="text" id="Name" name="Name" value='<%=Model.Name %>' class="textbox"/>
             </td>
        </tr>
        <tr>
            <td style="width: 200px;" ><%=Resources.Resource.title %></td>
            <td><%=ViewData["tName"]%></td>
        </tr>
        <tr>
            <td style="width: 200px;" ><%=Resources.Resource.edition %></td>
            <td><%=ViewData["shName"]%></td>
        </tr>
        <tr>
            <td style="width: 200px;" ><%=Resources.Resource.file %></td>
            <td>
                <input type="file" name="uploadFile" id="uploadFile"/>
            </td>
        </tr>
        <tr>
            <td style="width: 200px;" >&nbsp;</td>
            <td>
            <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
             <button name="button" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Submit1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
                
        
            </td>
        </tr>
    </table>
<%} %>

 </div></div>
       <div class='formBoxFooter'></div>
       </div>
       
       <script type="text/javascript">

           (function($) {
               $(document).ready(function() {
                   $("#SaveButton").click(function() {
                       var value = $("#Name").val();
                       if (value == '' || value == undefined || value == null) return false;
                       var fvalue = $("#uploadFile").val();
                       if (fvalue == '' || fvalue == undefined || fvalue == null) return false;
                       $("#form1").submit();

                   });

               });

           })(jQuery);
      
   </script>
   
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
