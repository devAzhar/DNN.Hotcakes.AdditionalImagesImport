<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Hotcakes.Modules.ProcessAdditionalImagesModule.SettingsView" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Import namespace="DotNetNuke.Services.Localization" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead">
    <a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a>
</h2>
<fieldset runat="server" id="fldSettings">
    <div class="dnnFormItem">
        <dnn:Label ID="lblScheduledJob" runat="server" /> 
        <asp:CheckBox ID="chkScheduledJob" runat="server" />
    </div>
</fieldset>
<div id="divMessage" class="dnnFormMessage dnnFormWarning" runat="server">
    <%=HostOnlyMessage %>
</div>