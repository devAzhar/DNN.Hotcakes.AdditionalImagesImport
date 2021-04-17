<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Hotcakes.Modules.ProcessAdditionalImagesModule.View" %>

<h1 class="text-center"><%=GetLocalizedString("ProcessImagesTitle") %></h1>
<p class="text-center"><%=GetLocalizedString("ProcessImagesMessage") %></p>

<p class="text-center">
    <asp:Button runat="server" ID="btnProcessImages" CssClass="dnnPrimaryAction" OnClick="lnkProcessImages_OnClick" CausesValidation="false"/>
    &nbsp;
    <asp:Button runat="server" ID="btnProcessAdditionalImages" CssClass="dnnPrimaryAction" OnClick="btnProcessAdditionalImages_OnClick" CausesValidation="false"/>
</p>

<asp:Panel runat="server" ID="pnlImportSummary">
    <asp:PlaceHolder runat="server" ID="phImportSummary"/>
</asp:Panel>