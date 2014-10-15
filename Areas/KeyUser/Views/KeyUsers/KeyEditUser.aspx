<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/KeyUser/Views/Shared/KeyUser.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.User>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script src="../../../../Scripts/js/jquery-ui-1.7.2.custom/js/jquery-1.3.2.min.js" type="text/javascript"></script>
<script src="../../../../Scripts/js/jquery-ui-1.7.2.custom/js/jquery-ui-1.7.2.custom.min.js" type="text/javascript"></script>

<div class='formBox'>
    <div class='formBoxHeader'>
        <div class='formBoxHeaderLabel'><%=Resources.Resource.userHeader %></div>
     </div>
     <div class='formBoxBody'>
        <div class='formBoxBodyInner'> 
       
        <% using (Html.BeginForm(new { userId = Model.UserId, controller = "KeyUsers", action = "EditUser" }))
          {%>
            <div id="tabs">
	            <ul>
		            <li><a href="#tabs-1"><%=Resources.Resource.overAll %></a></li>
		            <li><a href="#tabs-2"><%=Resources.Resource.titles %></a></li>
            		
	            </ul>
	         
	           
	                <div id="tabs-1">
		                <table cellpadding="2" cellspacing="0" width="100">
                            <thead></thead>
                          <tbody>
                          
                              <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.firstName )%>
                                    </td>
                                    <td>
                                        <input type="text" id="FirstName" name="FirstName" value ="<%= Model.FirstName%>"/>
                                    </td>
                                    
                               </tr>
                               
                                <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.middleName)%>
                                    </td>
                                    <td>
                                        <input type="text" name="MiddleName" id="MiddleName" value ="<%= Model.MiddleName%>"/>
                                    </td>
                                    
                               </tr>
                               
                               <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.surName)%>
                                    </td>
                                    <td>
                                        <input type="text" name="LastName" id="LastName" value ="<%= Model.LastName%>"/>
                                    </td>
                                    
                               </tr>
                               
                                <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.organisation)%>
                                    </td>
                                    <td>
                                        <%= Html.DropDownList("Organisation", "["+Resources.Resource.selectOrganisation+"]")%>
                                    </td>
                                    
                               </tr>
                               
                                <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.emailAddress)%>
                                    </td>
                                    <td>
                                        <input type="text" name="Email" id="Email" value ="<%=Model.Membership.Email%>"/>
                                    </td>
                                    
                               </tr>
                               
                               <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.userName)%>
                                    </td>
                                    <td>
                                        <input type="text" name="Username" id="Username" value ="<%= Model.Membership.Username%>"/>
                                    </td>
                                    
                               </tr>
                               
                               <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.password)%>
                                    </td>
                                    <td>
                                        <input type="password" id="Password" name="Password" />
                                    </td>
                                    
                               </tr>
                               
                               <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.lockedOut)%>
                                    </td>
                                    <td>
                                        <%= Html.CheckBox("LockedOut",Model.Membership.LockedOut)%>
                                        
                                    </td>
                                    
                               </tr>
                               
                               <tr>
                                    <td>
                                        <%= Html.Encode(Resources.Resource.role)%>
                                        
                                    </td>
                                    <td>
                                         <%= Html.DropDownList("Rol")%>
                                 
                                    </td>
                                    
                               </tr>
                               
                          </tbody>
                                                                            
                        </table>
                              
	                </div>
	
	                <div id="tabs-2">
                	
	                <%if (Model != null)
                   {%>
                       <%foreach( iZINE.Web.MVC.Models.TitleModel tille in (List<iZINE.Web.MVC.Models.TitleModel>)ViewData["Titles"])
                         {%>
                         
                         <%= Html.CheckBox(tille.TitleId.ToString(),tille.Seleceted)%>&nbsp;<%= tille.Name %><br />
                         <%} %>
                   
	                <%} %>
                	
	               </div>
	

</div>	
&nbsp;&nbsp;
    <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
    <button name="button" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Button1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
  
	<%} %>	
</div>
</div>
<div class="formBoxFooter"></div>
</div>
   <script type="text/javascript">
       $(function() {
           $("#tabs").tabs();
       });


       (function($) {
           $(document).ready(function() {
               $("#SaveButton").click(function() {
                   var sel = $('#Organisation option:selected').val();
                   if (sel == '' || sel == undefined || sel == null) return false;

                   var fname = $('#FirstName').val();
                   if (fname == '' || fname == undefined || fname == null) return false;

                   var sel2 = $('#Rol option:selected').val();
                   if (sel2 == '' || sel2 == undefined || sel2 == null) return false;


                   $("#form1").submit();

               });

           });

       })(jQuery);

	</script>

</asp:Content>
