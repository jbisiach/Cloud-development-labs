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

// The hotel reservation worker roel listens for messages in the 'hoteloffer' queue
// When it gets a message it checks and parses the content, then sends the result back to 'return-hoteloffer'

namespace hotel_res_service
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
            // Retrieve storage account from the connection string
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Setting2"));

            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);
            
            // Web Storage
            // storageAccount = CloudStorageAccount.Parse(@"UseDevelopmentStorage=true"); 
            // Local Storage

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            offerQueue = queueClient.GetQueueReference("hoteloffer");

            // Create the queue if it doesn't already exist
            offerQueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            returnOfferQueue = queueClient.GetQueueReference("return-hoteloffer");

            returnOfferQueue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("hotel_res_service is running");

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
            Trace.TraceInformation("Queues created");
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("hotel_res_service has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("hotel_res_service is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("hotel_res_service has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                // Get messages from the queue
                CloudQueueMessage input = offerQueue.GetMessage();

                if (input != null)
                {
                    Debug.WriteLine(input.AsString);

                    // Separate the message into parts
                    string[] separate = input.AsString.Split('|');
                    double price = 0;
                    int nbrTravellers = Int32.Parse(separate[2]);
                    int nbrSeniors = Int32.Parse(separate[3]);
                    int nbrNights = Int32.Parse(separate[4]);
                    int travellerNoSenior = nbrTravellers - nbrSeniors;

                    if (separate[0].Equals("1"))
                    {

                        for (int i = 0; i < travellerNoSenior; i++)
                        {
                            price += 600 * nbrNights;
                        }

                        for (int i = 0; i < nbrSeniors; i++)
                        {
                            price += 300 * nbrNights;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < travellerNoSenior; i += 2)
                        {
                            price += 900 * nbrNights;
                        }

                        for (int i = 0; i < nbrSeniors; i += 2)
                        {
                            price += 450 * nbrNights;
                        }
                    }

                    Debug.WriteLine(price.ToString("N2"));

                    offerQueue.DeleteMessage(input);

                    CloudQueueMessage returnMessage = new CloudQueueMessage(price.ToString("N2"));
                    returnOfferQueue.AddMessage(returnMessage);

                }
                await Task.Delay(1000);
            }
        }
    }
}
