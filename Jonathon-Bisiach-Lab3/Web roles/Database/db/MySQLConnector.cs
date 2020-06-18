using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace system_frontend.data.db
{

    // Simple MySQL server connector contains a single method to retrieve data from the server hosted in Azure Cloud
    // The data then can be easly bound to layout controls

    public class MySQLConnector
    {

        private MySqlConnection conn;
      
        public MySQLConnector()
        {

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "labserverv3.mysql.database.azure.com",
                Database = "mydb",
                UserID = "rootname@labserverv3",
                Password = "nameroot!1",
                SslMode = MySqlSslMode.Required,
            };

            this.conn = new MySqlConnection(builder.ConnectionString);
        }

        public IDataReader getData(String query)
        {
            MySqlDataReader reader;
            using (conn)
            {
                Console.WriteLine("Opening connection");
                conn.Open();

                using (var cmd = new MySqlCommand(query, conn))
                {
                    reader = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(reader);
                    return dt.CreateDataReader();
                }
            }

            return null;
        }
    }
}