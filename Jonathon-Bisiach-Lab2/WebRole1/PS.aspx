<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PS.aspx.cs" Inherits="WebRole1.PS" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Payment Service</h1>
      

        <p>
            <asp:Label Text="Credit Card Number: " runat="server"></asp:Label>
            <asp:TextBox ID="CreditNbr" Text="" runat="server" Width="400"></asp:TextBox>
        </p>

        <p>
            <asp:Label Text="Cardholder Name: " runat="server"></asp:Label>
            <asp:TextBox ID="CardHolder" Text="" runat="server" Width="400"></asp:TextBox>
           
            
        </p>

        

        <p>
            <asp:Label ID="Cost" runat="server"></asp:Label>
        </p>

        <p>
            <asp:Button runat="server" OnClick="Pay" Text="Pay" Visible="true">
            </asp:Button>
        </p>

    </div>


  

</asp:Content>
