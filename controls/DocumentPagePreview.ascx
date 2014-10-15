<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="DocumentPagePreview" Codebehind="DocumentPagePreview.ascx.cs" %>
    
    <table cellpadding="2" cellspacing="0" width='120px'>
        <tr>
            <td colspan="2">
                &nbsp;
                <div runat="server" id='dHeading' style="width: 120px; height: 32px; overflow: hidden; color: Black; padding: 1px 0 1px 0; font-size:9px;">
                    <b><span runat="server" id='sNumber' />.  <span runat="server" id='sName' /></b></div>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left">                                                                
                <asp:HyperLink runat="server" ID="ThumbnailLinkButton" Height="160" Width="118" rel="lightbox[roadtrip]" >
                    <asp:Image runat="server" ID="PageImage"
                            BorderColor="Black" BorderWidth="1" BorderStyle="Solid" Height="160" Width="118" />
                </asp:HyperLink>
                </a>
            </td>
        </tr>
        <tr>
            <td align="right" colspan='2' style='padding-left:15px'>
                &nbsp;
                <div style='margin-top:-2px;'>                
                <asp:Image runat="server" ID="CheckImage" ImageUrl="~/images/check.png" Width="16" Height="16" />
                <asp:HyperLink ID="HyperLinkPDFPage" runat="server">
                    <asp:Image ID="Image2" runat="server" SkinID="PdfIcon" Width="16" Height="16" /></asp:HyperLink>
               </div>
            </td>            
        </tr>
    </table>

<asp:Panel runat="server" ID="ImagePanel" CssClass="popupdiv2" Width="555px" Height="95%"
    BorderColor="#a5bd31" BorderWidth="1" BorderStyle="Solid" Style="display: none"
    ScrollBars="Auto">
    <table width="100%">
        <tr>
            <td>
                <b>
                     <span runat="server" id='sNamePopUp' /></b>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Image runat="server" ID="PageImagePopUp" 
                    BorderColor="Black" BorderWidth="1" BorderStyle="Solid" Height="710" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Button runat="server" Text="Sluiten" ID="CloseButton" Height="20" Width="60" />
            </td>
        </tr>
    </table>
</asp:Panel>
<%--<ajaxToolkit:ModalPopupExtender ID="ImageModalPopupExtender" runat="server" PopupControlID="ImagePanel"
    BackgroundCssClass="popupbg" TargetControlID="ThumbnailLinkButton" CancelControlID="CloseButton" />--%>
