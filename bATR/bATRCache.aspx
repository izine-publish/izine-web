<%@ Page Language="C#" MasterPageFile="~/bATR/Masters/default.master" AutoEventWireup="true" CodeFile="bATRCache.aspx.cs" Inherits="bATR_bATRCache" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphDefaultContent" Runat="Server">
	<div class="introText">
		This page lists the currently cached objects including the time that it will expire and the size it is in memory.
	</div>
	
	<div>
		<table class="dataTable">
		<tr>
			<th>Date cached</th>
			<th>Time till expiration</th>
			<th>Item(key) cached</th>
			<th>Size</th>
		</tr>
		<asp:Repeater ID="rptCacheContents" runat="server" OnItemDataBound="rptCacheContents_ItemDataBound">
		<ItemTemplate>
		<tr>
			<td><asp:Literal ID="ltlDate" runat="server"></asp:Literal></td>
			<td><asp:Literal ID="ltlTimeLeft" runat="server"></asp:Literal></td>
			<td><asp:Literal ID="ltlItem" runat="server"></asp:Literal></td>
			<td><asp:Literal ID="ltlSize" runat="server"></asp:Literal></td>
		</tr>
		</ItemTemplate>
		<FooterTemplate>
		<tr>
			<td colspan="3" style="text-align: right;">Total: <asp:Literal ID="ltlItemCount" runat="server"></asp:Literal></td>
			<td><asp:Literal ID="ltlTotalSize" runat="server"></asp:Literal></td>
		</tr>
		</FooterTemplate>
		</asp:Repeater>
		</table>
	</div>

</asp:Content>

