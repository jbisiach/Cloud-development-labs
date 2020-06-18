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

namespace FRS
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("FRS is running");

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

            Trace.TraceInformation("FRS has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("FRS is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("FRS has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("offer");
            CloudQueue returnOffer = client.GetQueueReference("returnoffer");

            while (!cancellationToken.IsCancellationRequested)
            {
                // get messages from the queue
                CloudQueueMessage input = cloudQueue.GetMessage();
                if (input != null)
                {
                    Debug.WriteLine(input.AsString);

                    // separate message into parts
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

                    // The distance between origin and destination airports
                    double distance = 6371 * Math.Acos(Math.Sin(degreeToRadians(latitudeOrigin)) * Math.Sin(degreeToRadians(latitudeDest)) + Math.Cos(degreeToRadians(latitudeOrigin)) * Math.Cos(degreeToRadians(latitudeDest)) * Math.Cos(d));
                    Debug.WriteLine("Distance in km: " + distance);

                    // get number of travellers from message
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

                    cloudQueue.DeleteMessage(input);

                    CloudQueueMessage returnMessage = new CloudQueueMessage(fare.ToString("N2"));
                    returnOffer.AddMessage(returnMessage);
                }

                await Task.Delay(1000);
            }
        }

        private double degreeToRadians(double angleDegrees) { return (Math.PI / 180) * angleDegrees; }

    }
}

// FRS worker role is listening for messages in the 'offer' queue.
// When it gets a message it checks the content and performs all parsing and calculations, then sends the result back to 'returnoffer' queue.
