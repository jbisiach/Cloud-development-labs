using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using system_frontend.data.model;

namespace system_frontend.data.db
{
    // Simple connector class to conenct to Azure Cosmos DB which is noSql (it is saving documents, not tables)
    // Database queries work the same way as SQL or MySQL database
    
    public class CosmosConnector

    {
        private const string EndpointUrl = "https://nosqllab3.documents.azure.com:443/";
        private const string AuthorizationKey = "1QtoPABBPqKtqsnuusnPeNrRF8LuFPGNSuPzwArk8E763DAYnCg6iuCO8Gb05792ue7WeFPVF63Ov2p9S6fHtw==";
        private const string DatabaseId = "lab3cosmosdb";
        private const string ContainerId = "mycontainer";
        private CosmosClient cosmosClient;


        public CosmosConnector()
        {
            this.cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
        }
        private List<Airport> queryData(string sqlQuery)
        {
          

             var container = cosmosClient.GetContainer(DatabaseId, ContainerId);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);

            List<string> data = new List<string>();

            foreach(string s in container.GetItemQueryIterator<string>(queryDefinition))
            {
                data.Add(s);
            }
            return data;
        }

    }

}