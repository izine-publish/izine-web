<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.status>" %>
<%@ Import Namespace="MvcPaging"%>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.status%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
            
           <% using (Html.BeginForm("editstatus", "KeyTitleStatus", new
   {
       name = ViewData["Name"],
       page = ViewData["page"],
       statusid = ViewData["statusId"],
       controller = "KeyTitleStatus",
       action = "editstatus"

   }, FormMethod.Post))
              {%> 
            <div style="visibility: visible;" id="panel">
			
            <table width="100%">
                <tbody><tr>
                    <td><%=Resources.Resource.name%></td>

                    <td><input name="Name" value='<%=Model.name %>'  id="Name" class="textbox" style="width: 200px;" type="text"/>&nbsp;<%= Html.ValidationMessage("Name","* Required")%><%= Html.ValidationMessage("Name1","* Name must be unique.")%></td>
                </tr>
                
          
            </tbody></table>
            <br />
            <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
             <button name="button" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Submit1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
            
        
		</div>
		<%} %>
    
            </div>
             <div class="formBoxFooter"></div>
        </div>
        
     </div>

</asp:Content>
