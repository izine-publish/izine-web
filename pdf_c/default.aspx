<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="iZine.Web.PDF_C.Default" EnableEventValidation="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Namespace="iZINE.Web.Controls" TagPrefix="izine" %>
<%@ Register TagPrefix="uc" TagName="Edit" Src="~/pdf_c/edit.ascx" %>
<%@ Register Src="~/controls/progress.ascx" TagName="ProgressControl" TagPrefix="izine" %>
<%@ Register src="~/admin/controls/ConfirmDeleteLinkButton.ascx" tagname="ConfirmDeleteLinkButton" tagprefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <izine:FormBox runat="server" ID='fbCertifiedPDF' Title='Certified PDF'>
        <ContentTemplate>
        
                                            <asp:MultiView runat="server" ActiveViewIndex="0" ID="StatusMultiView" >
                <asp:View ID="DefaultStatusView" runat="server"></asp:View>
                <asp:View ID="SavedStatusView" runat="server">
                    <p id="info">Document saved.</p>
                </asp:View>
                <asp:View ID="DeletedStatusView" runat="server">
                    <p id="info">Document deleted.</p>
                </asp:View>
            </asp:MultiView>      
    <table width="100%">

            <asp:MultiView runat="server" ID="MultiView" ActiveViewIndex="0">
                <asp:View runat="server" ID="ListView">
        <tr>
            <td colspan="2">
                        &nbsp;titel: &nbsp;
                        <asp:DropDownList runat="server" ID="TitlesDropDownList" DataTextField="Name" DataValueField="ConstantId" AppendDataBoundItems ="false">
                        </asp:DropDownList>
                    <ajaxToolkit:CascadingDropDown
                    ID="CascadingDropDown1"
                    runat="server"
                    Category="titles"
                    TargetControlID="TitlesDropDownList"
                    PromptText="[selecteer een titel]"
                    LoadingText="Laden titels..."
                    ServicePath=""
                    ServiceMethod="GetTitles">
                    </ajaxToolkit:CascadingDropDown>
                        &nbsp;&nbsp;&nbsp;plank:&nbsp;
                        <asp:DropDownList runat="server" ID="ShelvesDropDownList" DataTextField="Name" DataValueField="ConstantId" AppendDataBoundItems ="false" OnSelectedIndexChanged="ShelvesDropDownList_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        
                        <ajaxToolkit:CascadingDropDown ID="CDD1" runat="server"
                            TargetControlID="ShelvesDropDownList"
                            Category="shelves"
                            PromptText="[selecteer een plank]"
                            LoadingText="[Laden plank...]"
                            ServicePath=""
                            ServiceMethod="GetShelves"
                            ParentControlID="TitlesDropDownList"
                            />&nbsp;
                                                    <asp:LoginView ID="LoginView1" runat="server">
                            <RoleGroups>
                                <asp:RoleGroup Roles="0D629CBA-F55C-461A-831E-58053DB7189F, 1313712B-7675-450A-A0D3-C774366DFE45">
                                    <ContentTemplate>
                                    <asp:Button runat="server" ID="CreateButton" OnClick="CreateButton_Click" Text="NIEUW" SkinID="DefaultButton"/>
                                    </ContentTemplate>
                                </asp:RoleGroup>
                            </RoleGroups>
                        </asp:LoginView>

</td>
</tr>
<tr>
    <td>&nbsp;</td>
</tr>
<tr>
<td>                        
	
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
                            <ContentTemplate>
                            <izine:GridView runat="server" RowCssClass="GridRow" DataKeyNames="AssetId" 
                            HoverRowCssClass="GridRowOver" PageSize="10" AllowPaging="true" PagerStyle-CssClass="paging"  
                            ID="AssetsGridView" AutoGenerateColumns="false" 
                            HeaderStyle-BackColor="#a5be31" Width="824px" BackColor="#EFF2BF" 
                            BorderColor="#a5bd31" BorderStyle="Solid" BorderWidth="1px"
                             GridLines="None" HeaderStyle-HorizontalAlign="Left" 
                             OnPageIndexChanging="AssetsGridView_PageIndexChanging"                              
                            >
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="50" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("Version.VersionId", "asset.axd?id={0}") %>'><asp:Image ID="Image2" runat="server" SkinID="PdfIcon" Width="16" Height="16" /></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="200" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderTemplate  >
                                            <bATR:bATRImage ID="BATRImage1" Style="h2" runat="server" Text="Document" Theme="default"  />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# Eval("Name") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="50" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderTemplate><bATR:bATRImage ID="BATRImage3" Style="h2" runat="server" Text="Versie" Theme="default"  /></HeaderTemplate>
                                        <ItemTemplate><%# Eval("Version.Number", "#{0}" ) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="100" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderTemplate><bATR:bATRImage ID="BATRImage3" Style="h2" runat="server" Text="Datum" Theme="default"  /></HeaderTemplate>
                                        <ItemTemplate><%# Eval("Version.Date", "{0:d-M-yyyy}" ) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="150" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderTemplate><bATR:bATRImage ID="BATRImage3" Style="h2" runat="server" Text="User" Theme="default"  /></HeaderTemplate>
                                        <ItemTemplate><%# Eval("Version.User.Fullname") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField >                                
                                    <ItemTemplate>
                                        <div style='float:right; padding-right:5px;'>                                    
                                            <asp:LinkButton ID="LinkButton1" runat="server" Text="bewerken" CommandName="edit"></asp:LinkButton>                                    
                                            |
                                            <uc:ConfirmDeleteLinkButton ID="ConfirmDeleteLinkButton1" runat="server" CommandName="Delete" />
                                        </div>
                                    </ItemTemplate>
                            </asp:TemplateField>
                                </Columns>
                            </izine:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ShelvesDropDownList" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
            </td>
        </tr>
        </asp:View>
        <asp:View runat="server" ID="EditView">
        <tr>
            <td colspan="2">  
              <uc:Edit ID="EditControl" runat="server" OnCanceled="EditControl_Cancel" OnSaved="EditControl_Save" />
            </td>
        </tr>
        </asp:View>
        </asp:MultiView>
    </table>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <izine:ProgressControl ID="ProgressControl1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>
</ContentTemplate>
    </izine:FormBox>    
</asp:Content>

