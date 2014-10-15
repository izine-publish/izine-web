<%--//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Title/EditTitle.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-06-02 16:23:49 +0530 (Wed, 02 Jun 2010) $

//    $Revision: 1289 $

//    $Author: ajay.chaudhari $

//    Description: view for edit title

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/AdminSite.Master" Inherits="System.Web.Mvc.ViewPage<iZINE.Businesslayer.Title>" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class='formBox'>
    <div class='formBoxHeader'>
        <div class='formBoxHeaderLabel'><%=Resources.Resource.titleHeader %></div>
     </div>
     <div class='formBoxBody'>
        <div class='formBoxBodyInner'> 
       
       <% using (Html.BeginForm("edittitle", "title", new
   {
       tname = ViewData["tName"],
       titleid = ViewData["titleId"],
       orgselected = ViewData["orgSelected"],
       controller = "title",
       action = "edittitle"

   }, FormMethod.Post, new { id = "form1" }))
          {%>
    
   <table  width="100%">
        <tr>
            <td ><%=Resources.Resource.name %></td>
            <td><input type="text" name="Name" id="Name" value='<%=Model.Name%>' class="textbox" style="width:200px"/></td>
        </tr>
        <tr>
             <td><%=Resources.Resource.organisation %></td>
            <td><%= Html.DropDownList("Organisation", null, "["+Resources.Resource.selectOrganisation+"]", new { @id = "Organisation", style = "width:204px" })%></td>
        </tr>
        <tr>
            <td valign="top"><%=Resources.Resource.status %></td>
            <td>
                <asp:Panel  runat="server" Height="120px" Width="95%" BorderWidth="1pt" BorderColor="Black" BorderStyle="Solid" ScrollBars="Vertical">&nbsp;&nbsp;
                        <table style="margin-left:30px">
                            <tr>
                                <td style="width:50%;height:auto;border:1px solid;">
                                    
                                             <ul style="margin:0 0 0 2px;list-style:none">
                                                <%-- <li>
                                                        <%=Html.CheckBox("CheckAll") %>&nbsp;&nbsp;<%="All" %>
                                                    </li>--%>
                                                <li>
                                                    &nbsp;&nbsp;<span style="font-weight:bold">Layout:</span>
                                                </li>
                                                
                                                 <%foreach(iZINE.Web.MVC.Models.StatusModel statusLayout in (List<iZINE.Web.MVC.Models.StatusModel>)ViewData["status"])
                                                    { %>
                                                        <%int iCount = 0,iInnerCount = 0; 
                                                        if (ViewData["statusByTitle"] != null){ %>
                                                        <%foreach (iZINE.Web.MVC.Models.TitleStatusModel titleStatus in (List<iZINE.Web.MVC.Models.TitleStatusModel>)ViewData["statusByTitle"])
                                                          {                                                                                                                     
                                                            if (titleStatus.StatusId == statusLayout.StatusId && titleStatus.Layout == true)
                                                            {
                                                                if (ViewData["usedStatus"] != null)
                                                                {
                                                                    foreach (string usedStatus in (string[])ViewData["usedStatus"])
                                                                    {
                                                                        if (ViewData["usedStatus"] != null)
                                                                        {
                                                                            Guid? temp = new Guid(usedStatus);
                                                                            if (temp != null && titleStatus.StatusId == temp)
                                                                            {%>
                                                                          <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                            <%= Html.CheckBox("SessionLayout-disabled:" + statusLayout.StatusId.ToString(), true, new { @class = "SessionLayout", disabled = true })%>&nbsp;&nbsp;<%= statusLayout.Name.ToString()%>&nbsp;&nbsp;                                
                                                                          </li>     
                                                                                                                       
                                                                    <%     iInnerCount = 1; break;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                if(iInnerCount == 0)
                                                                {%>
                                                                            <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                                <%= Html.CheckBox("SessionLayout:" + statusLayout.StatusId.ToString(), true, new { @class = "SessionLayout"})%>&nbsp;&nbsp;<%= statusLayout.Name.ToString()%>&nbsp;&nbsp;                                
                                                                            </li>
                                                                <%}
                                                                iCount = 1; 
                                                                break; 
                                                            }                                         
                                                          }%>
                                                          <%if (iCount == 0)
                                                            { %>
                                                            <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                <%= Html.CheckBox("SessionLayout:" + statusLayout.StatusId.ToString(), false, new { @class = "SessionLayout" })%>&nbsp;&nbsp;<%= statusLayout.Name.ToString()%>&nbsp;&nbsp;                                
                                                              </li>
                                                              <%iCount = 0; 
                                                            } 
                                                         } %>
                                                        
                                                  <%} %>
                                            </ul>
                        
                                </td>
                                <td style="width:50%;height:auto;border:1px solid;">
                                    
                                            <ul >
                                              <li style="width:40px;list-style:none">
                                                    &nbsp;&nbsp;<span style="font-weight:bold">Text:</span>
                                              </li>
                                              <%foreach(iZINE.Web.MVC.Models.StatusModel statusText in (List<iZINE.Web.MVC.Models.StatusModel>)ViewData["status"]){ %>
                                                 <%
                                                    int iCount = 0,iInnerCount = 0; 
                                                    if (ViewData["statusByTitle"] != null)
                                                    { %>
                                                    <%foreach (iZINE.Web.MVC.Models.TitleStatusModel titleStatus in (List<iZINE.Web.MVC.Models.TitleStatusModel>)ViewData["statusByTitle"])
                                                      {
                                                        if (titleStatus.StatusId == statusText.StatusId && titleStatus.text == true)
                                                          {
                                                              if (ViewData["usedStatus"] != null)
                                                              {
                                                                  foreach (string usedStatus in (string[])ViewData["usedStatus"])
                                                                  {
                                                                      if (ViewData["usedStatus"] != null)
                                                                      {
                                                                          Guid? temp = new Guid(usedStatus);
                                                                          if (temp != null && titleStatus.StatusId == temp)
                                                                          {%>
                                                                      
                                                                          <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                            <%= Html.CheckBox("SessionText-disabled:" + statusText.StatusId.ToString(), true, new { @class = "SessionText", disabled = true })%>&nbsp;&nbsp;<%= statusText.Name.ToString()%>&nbsp;&nbsp;                                
                                                                          </li>
                                                                          
                                                                      <%
                                                                            iInnerCount = 1;
                                                                            break;
                                                                          }
                                                                      }
                                                                  }
                                                              }
                                                              if (iInnerCount == 0)
                                                              {%>
                                                                      <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                            <%= Html.CheckBox("SessionText:" + statusText.StatusId.ToString(), true, new { @class = "SessionText"})%>&nbsp;&nbsp;<%= statusText.Name.ToString()%>&nbsp;&nbsp;                                
                                                                      </li>
                                                            <%
                                                              }
                                                                    iCount = 1; 
                                                                    break; 
                                                            }                                         
                                                      }%>
                                                        <%if (iCount == 0)
                                                            { %>
                                                                <li style="float:left;height:15px;padding-bottom:6px;width:auto;list-style:none">
                                                                    <%= Html.CheckBox("SessionText:" + statusText.StatusId.ToString(), false, new { @class = "SessionText" })%>&nbsp;&nbsp;<%= statusText.Name.ToString()%>&nbsp;&nbsp;                                
                                                                </li>
                                                            <%
                                                                iCount = 0; 
                                                            } 
                                                     } %>
                                                    
                                              <%} %>
                                         </ul>
                           
                                </td>
                            </tr>
                </table>                                                                                
                </asp:Panel>
            </td>
        </tr>     
    </table>
    &nbsp
    <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
    <button name="button" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Button1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" ><%=Resources.Resource.cancel.ToUpper()%></button>
             
<%} %> 
 </div></div>  
    <div class="formBoxFooter"></div>
      </div>

<script language="javascript" type="text/javascript">


    (function($) {
        $(document).ready(function() {
            $("#SaveButton").click(function() {
                var sel = $('#Organisation option:selected').val();
                if (sel == '' || sel == undefined || sel == null) return false;

                var title = $('#Name').val();
                if (title == '' || title == undefined || title == null) return false;

                $("#form1").submit();

            });
//            $("#CheckAll").click(function() {
//                var checked_status = this.checked;
//                $(".Session").each(function() {
//                    this.checked = checked_status;                    
//                });
//               });

//            });
        });

    })(jQuery);
    
    
  </script>
</asp:Content>

