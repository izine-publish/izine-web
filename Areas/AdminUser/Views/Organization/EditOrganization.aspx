<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/AdminUser/Views/Shared/AdminUser.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.Organization>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.organisationHeader%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
            
           <% using (Html.BeginForm("editorganisation", "Organization", new
   {
       orgname = ViewData["OrgName"],
       page = ViewData["page"],
       orgid = ViewData["OrgId"],
       controller = "Organization",
       action = "editorganisation"

   }, FormMethod.Post))
              {%> 
            <div style="visibility: visible;" id="panel">
			
            <table width="100%">
                <tbody><tr>
                    <td><%=Resources.Resource.name%></td>

                    <td><input name="Name" value='<%=Model.Name %>'  id="Name" class="textbox" style="width: 200px;" type="text"/><%= Html.ValidationMessage("Name","* Required")%></td>
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
