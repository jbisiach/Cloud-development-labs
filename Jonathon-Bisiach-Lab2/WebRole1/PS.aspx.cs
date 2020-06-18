using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class PS : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue returnQueue = client.GetQueueReference("return-payment");
            CloudQueueMessage receive = null;

            // continues message received
            while (receive == null)
            {
                receive = returnQueue.GetMessage();
                Task.Delay(1000);
            }

            returnQueue.DeleteMessage(receive);
            // attach price to the label
            Cost.Text = receive.AsString;
        }

        public void Pay(object sender, EventArgs e)
        {
            // method is envoked when the button 'Pay' is pressed
            // method deliberately empty
        }
    }
    }