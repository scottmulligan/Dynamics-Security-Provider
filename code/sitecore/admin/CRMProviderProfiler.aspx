<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CRMProviderProfiler.aspx.cs" Inherits="CRMSecurityProvider.CRMProviderProfiler" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Performance profile for CRM Security Provider</title>
    </head>
    <body>
        <form id="form" runat="server">
            <h1>Performance profile for CRM Security Provider</h1>
            <asp:PlaceHolder ID="statistics" runat="server" />
            <asp:Button id="update" runat="server" OnClick="Update_Click" Text="Update" />
            <asp:Button id="reset" runat="server" OnClick="Reset_Click" Text="Reset" />
        </form>
    </body>
</html>