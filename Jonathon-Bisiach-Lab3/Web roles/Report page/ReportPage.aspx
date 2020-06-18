<%@ Page Title="Report " MasterPageFile="~/Site.Master" Language="C#" AutoEventWireup="true" CodeBehind="ReportPage.aspx.cs" Inherits="system_frontend.ReportPage" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Your contact page.</h3>

      <asp:Label Text="List of airports " runat="server"></asp:Label>

    <asp:GridView ID="airport" runat="server" Width="100"> 
</asp:GridView>  

     <asp:Label Text="List of airlines " runat="server"></asp:Label>

    <asp:GridView ID="airline" runat="server" Width="100%"> 
   
        </asp:GridView>  

          <asp:Label Text="Routes per airport  " runat="server"></asp:Label>
     <asp:DropDownList ID="DdlAirports" runat="server"  
                   AutoPostBack="True" onselectedindexchanged="AirportChanged">
            </asp:DropDownList>        

    <asp:GridView ID="route" AutoGenerateColumns="false" runat="server" Width="100%"> 
 <Columns>
     <asp:BoundField DataField="city" HeaderText="Departure Airport City" />
     <asp:BoundField DataField="symbol" HeaderText="Airport Symbol" />
     <asp:BoundField DataField="" HeaderText="Airport Symbol" />
 </Columns>

        </asp:GridView>  

          <asp:Label Text="List of hotels " runat="server"></asp:Label>

    <asp:GridView ID="hotel" runat="server" Width="100%"> 
        </asp:GridView>  

        <asp:Label Text="List of hotels per country " runat="server"></asp:Label>
    <asp:DropDownList ID="DdlCountry" runat="server"  onselectedindexchanged="CountryChanged">
            </asp:DropDownList>        
    <asp:GridView ID="percountry" runat="server" Width="100%"> 
        </asp:GridView>  

      <asp:Label Text="List of card holders " runat="server"></asp:Label>
      <asp:GridView ID="cardHolderList" runat="server" Width="100%"> 
        </asp:GridView>  

     <asp:Label Text="Transaction per holder " runat="server"></asp:Label>
      <asp:DropDownList ID="DdlHolder" runat="server"  onselectedindexchanged="HolderChanged">
            </asp:DropDownList>    
    
     <asp:GridView ID="transactions" runat="server" Width="100%"> 
        </asp:GridView>  


</asp:Content>

