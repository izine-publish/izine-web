<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="iZine.Web.PDF_C.Controls.Edit" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<table width="100%" cellpadding="5" cellspacing="10" style="">
        <tr>
            <td style="width: 200px;" class="FieldLabel">Naam</td>
            <td>
                <asp:TextBox ID="NameTextBox" runat="server"></asp:TextBox>
                <izine:HighlightExtender ID="HighlightExtender2" HoverClass="hover" runat="server" TargetControlID="NameTextBox"></izine:HighlightExtender>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NameTextBox"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="width: 200px;" class="FieldLabel">Titel</td>
            <td><asp:Label runat="server" ID="TitleLabel" /></td>
        </tr>
        <tr>
            <td style="width: 200px;" class="FieldLabel">Editie</td>
            <td><asp:Label runat="server" ID="ShelveLabel" /></td>
        </tr>
        <tr>
            <td style="width: 200px;" class="FieldLabel">Bestand</td>
            <td>
                <asp:FileUpload ID="FileUpload1" runat="server" CssClass="textbox"  />
            </td>
        </tr>
        <tr>
            <td style="width: 200px;" class="FieldLabel">&nbsp;</td>
            <td>
                    <asp:Button runat="server" ID="SaveButton" Text="OPSLAAN" OnClick="SaveButton_Click" SkinID="DefaultButton" />
                    <asp:Button runat="server" ID="CancelButton" Text="ANNULEREN" OnClick="CancelButton_Click" CausesValidation="false" SkinID="DefaultButton"/>
            </td>
        </tr>
    </table>
                            
