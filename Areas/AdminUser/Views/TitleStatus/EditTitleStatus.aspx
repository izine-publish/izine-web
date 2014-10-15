<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/AdminUser/Views/Shared/AdminUser.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Web.MVC.Areas.AdminUser.Models.EditTitleStatusModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class='formBox'>
        <div class='formBoxHeader'>
            <div class='formBoxHeaderLabel'><%=Resources.Resource.status%></div>
        </div>
        <div class='formBoxBody'>
            <div class='formBoxBodyInner'> 
            
           <% using (Html.BeginForm("editstatus", "titlestatus", new
   {
       page = ViewData["page"],
	   statusId = Model.StatusId,
       controller = "titlestatus",
       action = "editstatus"

   }, FormMethod.Post))
              {%> 
            <div style="visibility: visible;" id="panel">
			
            <table width="100%">
                <tbody><tr>
                    <td><%=Resources.Resource.name%></td>

                    <td><%=Html.TextBoxFor(n => n.Name, new { id="Name", @class="textbox" ,style="width: 200px;"})%>&nbsp;<%= Html.ValidationMessage("Name","* Required")%><%= Html.ValidationMessage("Name1","* Name must be unique.")%></td>
                    </tr>
                    <tr>
                    <td><%=Resources.Resource.State%></td>
                    <td>
						<%=Html.DropDownListFor(m=>m.StateId,new SelectList(Model.States,"StateId","Name") )%>
                    </td>
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
