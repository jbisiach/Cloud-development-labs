<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebRole1.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Travel Reservation System (TRS)</h3>
    <p>The Travel Reservation System (TRS) web application start page is the Flight Reservation Service (FRS), from which a customer can select the destination airport, the number of travellers and whether any of these passengers are eligible for any type of concession.  User can also select from this page whether they wish to rent accommodation or a hire-car. 

If these options are selected, users are taken to separate pages can select the number of beds they require and/or the size and type of hire-car.

Depending on the selections made, the total price is calculated in one or more of the worker roles and presented to the user in the final page.

This application uses six queues (emulated), with two for each web role.  One queue will be used to return the result back to user interface (UI) and one queue is used to store the data from the UI so that the price calculation can be performed in the worker role.  Three worker roles and three web roles are used: one each for the  FRS, Hotel Reservation Service (HRS) and Car Rental Service (CS), and for the Payment Service (PS).   
</p>
</asp:Content>
