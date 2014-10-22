<%--
//---------------------------------------------------------------------------------------------------------------
//    $HeadURL: https://my.redfive.biz/svn/izine/iZINE.Web/trunk/iZINE.Web.MVC/Views/Plank/DocumetPagePreview.ascx $

//   Owner: Prakash Bhatt

//    $Date: 2010-02-01 16:44:01 +0530 (Mon, 01 Feb 2010) $

//    $Revision: 802 $

//    $Author: prakash.bhatt $

//    Description: view for document page preview

//   Copyright: 2010 iZine Solutions. All rights reserved.
//---------------------------------------------------------------------------------------------------------------
--%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DocumentPagePreviewData>" %>
 
    <table cellpadding="0" cellspacing="0" >
        <tbody>
            <tr>
                <td colspan="2">
                    &nbsp;
                    <div id="dHeading" style="padding: 1px 0pt; overflow: hidden; width: 100px; height: 20px; color: Black; font-size: 9px ; padding:1px 0 1px 0;">
                        <b><span id="sNumber"><%=Model.Number.ToString() %></span>.  <span id="sName" title='<%=Model.Number %>'><%=Model.Name %></span></b></div>
                </td>

            </tr>
            <tr>
                <td colspan="2" align="left">                                                                
                    <a id="ThumbnailLinkButton" rel="lightbox[roadtrip]" title="<%=Model.Name %>" href='<%= Html.Encode("/image.axd?id="+Model.PageId)%>' style="display:inline-block; height: 162px; width: 100px;">
                    <img id="PageImage" src='<%= Html.Encode("/image.axd?id="+Model.PageId)%>' style="border: 1px solid Black; height: 160px; width: 100px; text-align:right" alt="" /></a>
                    
                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding-left: 15px;" align="right">
                 <div >  
                  <%if (new Guid("22F8ACBD-8E7C-4437-A767-E5773DB76083").CompareTo(Model.StatusId) == 0)
                    { %> 
                    <img id="CheckImage" src="/images/check.png" width="16" height="16" alt=""/>  
                    <%} %>             
                    <a id="HyperLinkPDFPage" href='<%= Html.Encode("/pdf.axd?id="+Model.VersionId)%>'><img id="Image2" src="../../App_Themes/green/images/pdf_icon.png" style="border-width: 0px; height: 16px; width: 16px;" alt=""/></a>
                   </div>
                </td>            
            </tr>
    </tbody>
 </table>
    




