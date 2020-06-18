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

namespace HRS
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("HRS is running");

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

            Trace.TraceInformation("HRS has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HRS is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HRS has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("hoteloffer");
            CloudQueue returnOffer = client.GetQueueReference("return-hoteloffer");
            while (!cancellationToken.IsCancellationRequested)
            {

                // get messages from the queue
                CloudQueueMessage input = cloudQueue.GetMessage();

                if (input != null)
                {
                    Debug.WriteLine(input.AsString);

                    // separate message into parts
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

                    cloudQueue.DeleteMessage(input);

                    CloudQueueMessage returnMessage = new CloudQueueMessage(price.ToString("N2"));
                    returnOffer.AddMessage(returnMessage);
                }

                await Task.Delay(1000);
            }
        }
    }
}

// HRS worker role is listening for messages in the 'hoteloffer' queue.
// When a message is received it checks the content and performs all parsing and calculations.
// Then it sends the result back to 'return-hoteloffer' queue.