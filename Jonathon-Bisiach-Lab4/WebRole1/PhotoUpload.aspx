<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhotoUpload.aspx.cs" Inherits="WebRole1.PhotoUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form runat="server">

        <p>Add photos </p>

        <asp:FileUpload ID="FileUpload" runat="server" />
        <asp:Button ID="Upload" runat="server" OnClick="OnUploadClick" Text="Upload" />

        <asp:TextBox ID="Title" runat="server" Text="Title of the photo!"> </asp:TextBox>
        <asp:TextBox ID="Description" runat="server" Text="Description of the photo!"> </asp:TextBox>


    </form>



</body>
</html>
