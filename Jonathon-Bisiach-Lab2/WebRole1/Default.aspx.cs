using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Offer(object sender, EventArgs e)
        {
            string startAirport = DdlStartAirport.SelectedValue;
            string destAirport = DdlDestinationAirport.SelectedValue;
            string infants = Infants.Text;
            string children = Children.Text;
            string adults = Adults.Text;
            string seniors = Seniors.Text;

            string send = startAirport + "|" + destAirport + "|" + infants + "|" + children + "|" + adults + "|" + seniors;

            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("offer");
            CloudQueueMessage message = new CloudQueueMessage(send);
            cloudQueue.AddMessageAsync(message);

            CloudQueue returnQueue = client.GetQueueReference("returnoffer");
            CloudQueueMessage receive = null;

            // continue until we get a message
            while (receive == null)
            {
                receive = returnQueue.GetMessage();
                Task.Delay(1000);

            }
            returnQueue.DeleteMessage(receive);
            // put the price to the label
            Price.Text = receive.AsString;
            BtnContinue.Visible = true;

        }

        public void Continue(object sender, EventArgs e)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("payment");


            // if hotel checkbox is checked then the price being sent to the payment service is not the last one ( indicated by number 0)
            if (HotelCheck.Checked)
            {
                CloudQueueMessage message = new CloudQueueMessage(Price.Text + "|0");
                cloudQueue.AddMessageAsync(message);
                Response.Redirect("HRS.aspx");
            }
            // if hotel checkbox is not checked then the price being sent to the payment service is the last one ( indicated by number 1)
            else
            {
                CloudQueueMessage message = new CloudQueueMessage(Price.Text + "|1");
                cloudQueue.AddMessageAsync(message);
                Response.Redirect("PS.aspx");
            }

        }
    }


}