<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Flights.aspx.cs" Inherits="WebRole1.Flights" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
 
    <div class="jumbotron">
        <h1 style="color:blue;">Flight Reservation Service </h1>

       
    </div>
    
     <p style="background-color:powderblue;">
        
            <b><u>Hotel Reservation:</u></b>
            <asp:CheckBox ID="HotelCheck" runat="server" />
        </p>

    <p>
          
            <asp:DropDownList ID="DdlStartAirport" runat="server">
                <asp:ListItem Enabled="true" Text="Departure Airport" Value="-1" ></asp:ListItem>
                <asp:ListItem Text="Stockholm Arlanda (STO)" Value="STO"></asp:ListItem>
                <asp:ListItem Text="Copenhagen Kastrup (CPH)" Value="CPH"></asp:ListItem>
                <asp:ListItem Text="Frankfurt (FRA)" Value="FRA"></asp:ListItem>
                <asp:ListItem Text="Paris Charles De Gaulle (CDG)" Value="CDG"></asp:ListItem>
                <asp:ListItem Text="London Heathrow (LHR)" Value="LHR"></asp:ListItem>
            </asp:DropDownList>        
          </p>

            <p>
            <asp:DropDownList ID="DdlDestinationAirport" runat="server">
                <asp:ListItem Enabled="true" Text="Destination airport" Value="-1"></asp:ListItem>
                <asp:ListItem Text="Stockholm Arlanda (STO)" Value="STO"></asp:ListItem>
                <asp:ListItem Text="Copenhagen Kastrup (CPH)" Value="CPH"></asp:ListItem>
                <asp:ListItem Text="Frankfurt (FRA)" Value="FRA"></asp:ListItem>
                <asp:ListItem Text="Paris Charles De Gaulle (CDG)" Value="CDG"></asp:ListItem>
                <asp:ListItem Text="London Heathrow (LHR)" Value="LHR"></asp:ListItem>
        </asp:DropDownList>        
        </p>

    <p style="background-color:powderblue;">
            <asp:Label Text="Infants(<2)" runat="server" Width="120px"></asp:Label>
            <asp:Label Text="Children(<13)" runat="server" Width="130px"></asp:Label>
            <asp:Label Text="Adults" runat="server" Width="130px"></asp:Label>
            <asp:Label Text="Seniors(>65)" runat="server" Width="130px"></asp:Label>
        </p>
        <p>
            <asp:TextBox ID="Infants" Text="0" runat="server" Width="115px"></asp:TextBox>
            <asp:TextBox ID="Children" Text="0" runat="server" Width="115px"></asp:TextBox>
            <asp:TextBox ID="Adults" Text="0" runat="server" Width="115px"></asp:TextBox>
            <asp:TextBox ID="Seniors" Text="0" runat="server" Width="115px"></asp:TextBox>
        </p>


        <p>
            <asp:Button ID="BtnGet" runat="server"  Text="Get Offer" Visible="false">
            </asp:Button>
        </p>

        <p>
            <asp:Label ID="Price" runat="server"></asp:Label>

        </p>

        <p>
            <asp:Button ID="BtnContinue" runat="server"  Text="Continue" Visible="false">
            </asp:Button>
        </p>




  

</asp:Content>
