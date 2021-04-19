<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.ProcessAdditionalImagesModule.SettingsView" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Import namespace="DotNetNuke.Services.Localization" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead">
    <a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a>
</h2>
<fieldset runat="server" id="fldSettings">
    <div class="dnnFormItem">
        <dnn:Label ID="lblDownloadFolderPath" runat="server" /> 
        <asp:TextBox ID="txtDownloadsFolderPath" runat="server" />
    </div>
</fieldset>

<fieldset runat="server" id="fldSettings2">
    <div class="dnnFormItem">
        <dnn:Label ID="lblAdditionalFolderPath" runat="server" /> 
        <asp:TextBox ID="txtAdditionalFolderPath" runat="server" />
    </div>
</fieldset>

<div id="divMessage" class="dnnFormMessage dnnFormWarning" runat="server">
    <%=HostOnlyMessage %>
</div>