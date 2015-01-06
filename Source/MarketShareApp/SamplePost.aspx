<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SamplePost.aspx.cs" Inherits="SamplePost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
   <%-- <form id="form1" runat="server">
        <asp:HiddenField id="UserGuidField" value='<%# UserGuid.GetValue() %>' runat="server" />
        <asp:Button ID="Button1" runat="server" Text="Button" PostbackUrl ="http://marketshare.seeff.com/Default.aspx"/>
    </form>--%>

     <form id="form1" runat="server">
         <asp:HiddenField ID="UserGuidField" Value='<%# UserGuid.GetValue() %>' runat="server" />
        <asp:Button ID="Button1" runat="server" Text="Button" PostbackUrl ="http://marketshare.seeff.com/Default.aspx"/>
    </form>

</body>
</html>
