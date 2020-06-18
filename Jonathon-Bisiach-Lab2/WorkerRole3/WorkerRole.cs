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
using Microsoft.WindowsAzure.Storage.Queue;

namespace PS
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("PS is running");

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

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("PS has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("PS is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("PS has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            // 'payment' queue contains the prices from FRS and HRS
            CloudQueue Prices = client.GetQueueReference("payment");
            // 'return-payment' queue contains the total price
            CloudQueue totalPrice = client.GetQueueReference("return-payment");
            List<String> prices = new List<string>();
            while (!cancellationToken.IsCancellationRequested)
            {
                CloudQueueMessage input = Prices.GetMessage();

                if (input != null)
                {
                    string[] separate = input.AsString.Split('|');

                    // separate[0] is the price and the separate[1] is the one of binaries 0 or 1 
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

// PS worker role is listening for messages from the payment queue.
// When a message is received the content should include binaries(0,1) and the price.
// Binary 0 means that another prices is coming (when user checks the hotel reservation option in the FRS web role).
// Binary 1 means that the price is final with no more additional content, and the total amount is sent to the 'return-payment' queue.
