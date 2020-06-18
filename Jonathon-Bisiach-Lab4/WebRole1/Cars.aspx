<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cars.aspx.cs" Inherits="WebRole1.Cars" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Hotel Reservation Service</h1>
        <p class="lead">Pick your hotel below!</p>

        <p>Room type:
            <asp:DropDownList ID="RoomType" runat="server">
                <asp:ListItem Enabled="true" Text="Select room type" Value="-1"></asp:ListItem>
                <asp:ListItem Text="Single" Value="1"></asp:ListItem>
                <asp:ListItem Text="Double" Value="2"></asp:ListItem>
            </asp:DropDownList>        
        </p>


        <p>
            <asp:Label Text="Full name of primary guest: " runat="server"></asp:Label>
            <asp:TextBox ID="PrimaryGuest" Text="" runat="server" Width="400"></asp:TextBox>
        </p>

        <p>
            <asp:Label Text="Number of Travellers: " runat="server"></asp:Label>
            <asp:TextBox ID="Travellers" Text="" runat="server" Width="20"></asp:TextBox>
           
            
        </p>

        <p>
             <asp:Label Text=" Number of Seniors: " runat="server"></asp:Label>
            <asp:TextBox ID="Seniors" Text="" runat="server" Width="20"></asp:TextBox>
        </p>
        <p>
            <asp:Label Text="Number of Nights: " runat="server"></asp:Label>
            <asp:TextBox ID="Nights" Text="" runat="server" Width="20"></asp:TextBox>

        </p>


        <p>
            <asp:Button ID="BtnGet" runat="server"  Text="Get Offer">
            </asp:Button>
        </p>

        <p>
            <asp:Label ID="Price" runat="server"></asp:Label>
        </p>

        <p>
            <asp:Button ID="goOn" runat="server"  Text="Continue" Visible="false">
            </asp:Button>
        </p>

    </div>


  

</asp:Content>
