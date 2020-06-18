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

// The payment worker role is listening for messages in the 'payment' queue.
// The message content includes 2 binary digits (0,1) and the price
// Digit 0 indicates that another prices is coming (when the hotel reservation checkbox is checked in the FRS)
// Digit 1 indicates no more payments are incoming
// When the final price is received the total price is calculated and sent to 'return-payment' queue.

namespace payment_service
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
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Setting2"));

            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true); 
            // Web Storage
            // storageAccount = CloudStorageAccount.Parse(@"UseDevelopmentStorage=true"); 
            // Local Storage

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            offerQueue = queueClient.GetQueueReference("payment");

            // Create the queue if it doesn't already exist
            offerQueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            returnOfferQueue = queueClient.GetQueueReference("return-payment");

            returnOfferQueue.CreateIfNotExists();

        }
        public override void Run()
        {
            Trace.TraceInformation("payment_service is running");

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

            Trace.TraceInformation("payment_service has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("payment_service is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("payment_service has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            // 'payment' queue contains the prices from FRS and HRS
            CloudQueue Prices = client.GetQueueReference("payment");
            // 'return-payment' queue contains the total price (FRS + HRS)
            CloudQueue totalPrice = client.GetQueueReference("return-payment");
            List<String> prices = new List<string>();
            while (!cancellationToken.IsCancellationRequested)
            {
                CloudQueueMessage input = Prices.GetMessage();

                if (input != null)
                {
                    string[] separate = input.AsString.Split('|');

                    // separate[0] is the price and the separate[1] is the one of two digits: 0 or 1 
                    if (separate[1].Equals("0"))
                    {
                        prices.Add(separate[0]);
                    }

                    if (separate[1].Equals("1"))

                    {
                        // add the price
                        prices.Add(separate[0]);
                        double total = 0;
                        foreach (String price in prices)
                        {
                            Debug.WriteLine(price);
                            // calculate total cost
                            total += Double.Parse(price);
                        }

                        CloudQueueMessage message = new CloudQueueMessage(total.ToString());
                        totalPrice.AddMessage(message);
                        prices = new List<string>();

                    }
                    Prices.DeleteMessage(input);
                }

                await Task.Delay(1000);
            }
        }
    }
}
