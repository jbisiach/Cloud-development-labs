<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPhotos.aspx.cs" Inherits="WebRole1.ViewPhotos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        
        <p> This table shows the title, description and owner of a photo</p>
        <asp:Table ID="table" runat="server" CellPadding="20" GridLines="Both" ></asp:Table>


    </form>
</body>
</html>
