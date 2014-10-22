<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Setting/default.aspx $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-25 12:00:04 +0100 (Thu, 25 Feb 2010) $

//    $Revision: 860 $

//    $Author: prakash.bhatt $

//    Description: View for settings

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MemberSite.Master" Inherits="System.Web.Mvc.ViewPage<List<iZINE.Web.MVC.Models.NotificationModel>>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class='formBoxHeader'>
        <div class='formBoxHeaderLabel'><%=Resources.Resource.emailSettings %></div>
    </div>
    
    <div class='formBoxBody' style="width:1000">
        <div class='formBoxBodyInner'> 
        <form id="form1" method="post" action="/setting/savenotification">
        <div style="background-color:#FFF2BF;width:810px">
        <table width="810px" style="border:solid 1px #A5BD31">
        <tr style="background-color:#A5BD31">
        <td><b><%=Resources.Resource.title.ToUpper() %></b></td>
        <%foreach (iZINE.Businesslayer.Constant st in (List<iZINE.Businesslayer.Constant>)ViewData["status"])
          { %>
        <td align="center"><b><%=st.Name%></b></td>
        <%} %>
        </tr>
        <% List<iZINE.Businesslayer.Title> tList = (List<iZINE.Businesslayer.Title>)ViewData["titel"];
            List<iZINE.Businesslayer.Constant> sList = (List<iZINE.Businesslayer.Constant>)ViewData["status"];
           for (int i = 0; i < tList.Count;i++ )
           { %>     
        <tr <%= i%2==0? "style='background-color:rgb(239, 242, 191)'":"" %>>
        <td >
            <b><%=tList[i].Name %></b>
        </td>
         <%for (int j = 0; j < sList.Count;j++ )
           {  %> 
        <td align="center"> <%= Html.CheckBox(sList[j].ConstantId + "+" + tList[i].TitleId, Model.FirstOrDefault(n => n.StatusId == sList[j].ConstantId && n.TitleId == tList[i].TitleId) == null ? false : true)%>
        <%--<input type="checkbox"  name='<%=sList[j].ConstantId+"+"+ tList[i].TitleId%>'<%=Model.FirstOrDefault(n=>n.StatusId == sList[j].ConstantId && n.TitleId== tList[i].TitleId)== null ? "":"checked" %> />
        --%></td>
        <%} %>
        </tr>
        <%} %>
        </table>
        </div>
        
         <input type="submit" name="SaveButton" value="<%=Resources.Resource.save.ToUpper()%>" id="SaveButton" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
         <input type="reset" value="<%=Resources.Resource.cancel.ToUpper()%>" id="Submit1" class="defaultButton" onmouseover="this.className='defaultButtonOver';" onmouseout="this.className='defaultButton';" />
           
        </form>
       </div>
       </div>
       <div class="formBoxFooter"></div>
   
</asp:Content>

