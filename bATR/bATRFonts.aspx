<%@ Page Title="" Language="C#" MasterPageFile="~/bATR/Masters/default.master" AutoEventWireup="true" CodeFile="bATRFonts.aspx.cs" Inherits="bATR_bATRFonts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphDefaultContent" Runat="Server">
	<div class="introText">
		This page lists the available fonts and types that are currently configured in the bATR Fonts 
		folder. You can use the names and styles for the bATR.Config.
	</div>
	
	<div>
		<table class="dataTable">
		<tr>
			<th>Font name</th>
			<th>Weight</th>
			<th>Style</th>
			<th>Stretch</th>
		</tr>
		<asp:Repeater ID="rptFonts" runat="server" OnItemDataBound="rptFonts_ItemDataBound">
		<ItemTemplate>
			<tr>
				<td><asp:Literal ID="ltlFontName" runat="server"></asp:Literal></td>
				<td><asp:Literal ID="ltlFontWeight" runat="server"></asp:Literal></td>
				<td><asp:Literal ID="ltlFontStyle" runat="server"></asp:Literal></td>
				<td><asp:Literal ID="ltlFontStretch" runat="server"></asp:Literal></td>
			</tr>
		</ItemTemplate>
		</asp:Repeater>
		</table>
	</div>
	
</asp:Content>

