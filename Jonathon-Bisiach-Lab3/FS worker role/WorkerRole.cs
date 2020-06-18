using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

// The flight reservation worker role is listening for messages in the 'offer' queue.
// When a message is received it checks and parses the content before sending the result back to 'returnoffer' queue.

namespace flight_res_service
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "mysql3";      // YOUR AZURE STORAGE ACCOUNT_NAME";             
        private string accountKey = "ou6lTDUrlvto31oVn9jqo1RtcE5GLdaGZqXmj/9/OLfXQIgSNaFlRojz31voeqHauvz3Vc/+b0AIoLK1ZtWU8Q==";     // zPie75n + Wcbwr19brs3LNC05ldiv4sDAPLB6ib4 / eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw ==";     // zPie75n+Wcbwr19bferrs3LNCdiv4sDAPsdLB6ib4/eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw";   // Write your Azure storage account key here "YOUR_ACCOUNT_KEY";     
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue offerQueue, returnOfferQueue;

        private void initQueue()
        {
            // Retrieve storage account from connection string
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            // CloudConfigurationManager.GetSetting("Setting2"));

            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true); //Web Storage
           // storageAccount = CloudStorageAccount.Parse(@"UseDevelopmentStorage=true"); //Local Storage


            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            offerQueue = queueClient.GetQueueReference("offer");

            // Create the queue if it doesn't already exist
            offerQueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            returnOfferQueue = queueClient.GetQueueReference("returnoffer");

            returnOfferQueue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("flight_res_service is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            initQueue();
            Trace.TraceInformation("Queues have been created");


            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("flight_res_service has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("flight_res_service is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("flight_res_service has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
          // CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
           
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                // Get messages from the queue
                CloudQueueMessage input = offerQueue.GetMessage();
                if (input != null)
                {
                    Debug.WriteLine(input.AsString);

                    // Separate the message
                    string[] separate = input.AsString.Split('|');

                    double latitudeOrigin = 0, latitudeDest = 0, longitudeOrigin = 0, longitudeDest = 0, baseRate = 0;

                    switch (separate[0])
                    {
                        case "STO":
                            latitudeOrigin = 59.6519;
                            longitudeOrigin = 17.9186;
                            baseRate = 0.234;
                            break;
                        case "CPH":
                            latitudeOrigin = 55.6181;
                            longitudeOrigin = 12.6561;
                            baseRate = 0.2554;
                            break;
                        case "CDG":
                            latitudeOrigin = 49.0097;
                            longitudeOrigin = 2.5478;
                            baseRate = 0.2255;
                            break;
                        case "LHR":
                            latitudeOrigin = 31.5497;
                            longitudeOrigin = 74.3436;
                            baseRate = 0.2300;
                            break;
                        case "FRA":
                            latitudeOrigin = 50.1167;
                            longitudeOrigin = 8.6833;
                            baseRate = 0.2400;
                            break;

                    }

                    switch (separate[1])
                    {
                        case "STO":
                            latitudeDest = 59.6519;
                            longitudeDest = 17.9186;

                            break;
                        case "CPH":
                            latitudeDest = 55.6181;
                            longitudeDest = 12.6561;

                            break;
                        case "CDG":
                            latitudeDest = 49.0097;
                            longitudeDest = 2.5478;

                            break;
                        case "LHR":
                            latitudeDest = 31.5497;
                            longitudeDest = 74.3436;

                            break;
                        case "FRA":
                            latitudeDest = 50.1167;
                            longitudeDest = 8.6833;

                            break;
                    }



                    double d = degreeToRadians(longitudeOrigin) - degreeToRadians(longitudeDest);
                    if (d < 0)
                    {
                        d *= -1;
                    }

                    // The distance between the origin and destination airports
                    double distance = 6371 * Math.Acos(Math.Sin(degreeToRadians(latitudeOrigin)) * Math.Sin(degreeToRadians(latitudeDest)) + Math.Cos(degreeToRadians(latitudeOrigin)) * Math.Cos(degreeToRadians(latitudeDest)) * Math.Cos(d));
                    Debug.WriteLine("Distance in km: " + distance);

                    // Get the amount of travellers from the message
                    int infants = Int32.Parse(separate[2]);
                    int children = Int32.Parse(separate[3]);
                    int adults = Int32.Parse(separate[4]);
                    int seniors = Int32.Parse(separate[5]);

                    double fare = 0;

                    for (int i = 0; i < infants; i++)
                    {
                        fare += baseRate * distance * (1 - 0.9);
                    }

                    for (int i = 0; i < children; i++)
                    {
                        fare += baseRate * distance * (1 - 0.33);
                    }

                    for (int i = 0; i < adults; i++)
                    {
                        fare += baseRate * distance * (1 - 0.00);
                    }

                    for (int i = 0; i < seniors; i++)
                    {
                        fare += baseRate * distance * (1 - 0.25);
                    }



                    Debug.WriteLine(fare.ToString("N2"));

                    offerQueue.DeleteMessage(input);

                    CloudQueueMessage returnMessage = new CloudQueueMessage(fare.ToString("N2"));
                    returnOfferQueue.AddMessage(returnMessage);

                }
                await Task.Delay(1000);
            }
        }
        private double degreeToRadians(double angleDegrees) { return (Math.PI / 180) * angleDegrees; }

    }
}
