using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using system_frontend.data.db;
using system_frontend.data.model;

namespace system_frontend
{

    // MySQLConnector or CosmosConnector used to retrieve data
    // Retrieved data is bound to the layout items like drop down lists or grid views in their respective methods
    // There are event trigger methods to detect changes in selected items in the drop down list 

    public partial class ReportPage : System.Web.UI.Page
    {

        private MySQLConnector mySQLConnector;
        private readonly String GET_AIRPORTS_QUERY = "SELECT * FROM airports;";
        private readonly String GET_AIRLINES_QUERY = "SELECT * FROM airlines; ";
        private readonly String GET_HOTELS_QUERY = "SELECT * FROM hotels;";
        private readonly String GET_CARDHOLDERS_QUERY = "SELECT * FROM customers;";
        private CosmosConnector cosmosConnector;

        protected void Page_Load(object sender, EventArgs e)
        {
            mySQLConnector = new MySQLConnector();
            cosmosConnector = new CosmosConnector();

            // List<string> airports =  cosmosConenctor.queryData("SELECT * FROM airports");
            // Servers automatically send back data after any change in drop down list occurs,
            // The page is loaded once only in initialisation in order to prevent resetting data
           
            if (!IsPostBack)
            {
                LoadAirportData();
                LoadAirlineData();
                LoadHotels();
                LoadCardHolders();
            }
        }

        private void LoadAirportData() {
            var dataReader = mySQLConnector.getData(GET_AIRPORTS_QUERY);
    
                airport.DataSource = dataReader;
                airport.DataBind();
           
            var dataReader2 = mySQLConnector.getData(GET_AIRPORTS_QUERY);
            DdlAirports.DataSource = dataReader2;
            DdlAirports.DataTextField = "city";
            DdlAirports.DataValueField = "code";
            DdlAirports.DataBind();               
        }

        private void LoadAirlineData() {
            var dataReader = mySQLConnector.getData(GET_AIRLINES_QUERY);
            if (dataReader.Read())
            {
                airline.DataSource = dataReader;
                airline.DataBind();
            }
        }

        private void LoadRoutesPerAirport()
        {
            var dataReader = mySQLConnector.getData("select * from routes join airports where routes.departure_airports_code " +
             "= airports.code AND airports.city = '" + DdlAirports.SelectedItem.Text + "' ;");
    
            route.DataSource = dataReader;
            route.DataBind();        
        }

        private void LoadHotels()
        {
            var dataReader = mySQLConnector.getData(GET_HOTELS_QUERY);
          
                hotel.DataSource = dataReader;
                hotel.DataBind();        
        }

        private void LoadHotelsPerCountry()
        {
            var dataReader = mySQLConnector.getData("SELECT * FROM hotels WHERE country='" +DdlCountry.SelectedItem.Text+ "';");
       
                percountry.DataSource = dataReader;
                percountry.DataBind();     
        }

        private void LoadCardHolders()
        {
            var dataReader = mySQLConnector.getData(GET_CARDHOLDERS_QUERY);
           
                cardHolderList.DataSource = dataReader;
                cardHolderList.DataBind();
           
            var dataReader2 = mySQLConnector.getData(GET_CARDHOLDERS_QUERY);

            DdlHolder.DataSource = dataReader2;
            DdlHolder.DataTextField = "name";
            DdlHolder.DataValueField = "card_number";
            DdlHolder.DataBind();

        }

        private void LoadTransactionsPerCardHolder()
        {
            var dataReader = mySQLConnector.getData("SELECT * FROM transactions WHERE customers_card_number=" +DdlHolder.SelectedItem.Value +";");
           
                transactions.DataSource = dataReader;
                transactions.DataBind();         
        }

        protected void AirportChanged(object sender, EventArgs e) {
           
            LoadRoutesPerAirport();        
        }

        protected void CountryChanged(object sender, EventArgs e)
        {
            LoadHotelsPerCountry();
        }

        protected void HolderChanged(object sender, EventArgs e)
        {
            LoadTransactionsPerCardHolder();
        }
    }
}
