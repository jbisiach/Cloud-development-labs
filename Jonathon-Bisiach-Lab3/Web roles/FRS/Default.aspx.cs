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

            // Continue until a message is received
            while (receive==null)
            {
               receive= returnQueue.GetMessage();
                Task.Delay(1000);

            }
            returnQueue.DeleteMessage(receive);

            // Send price to the label
            Price.Text = receive.AsString;
            BtnContinue.Visible=true;

        }

        public void Continue(object sender, EventArgs e)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("payment");
            
           
            // If the hotel check box is selected, binary 0 is attached
            if (HotelCheck.Checked)
            {
                CloudQueueMessage message = new CloudQueueMessage(Price.Text+"|0");
                cloudQueue.AddMessageAsync(message);
                Response.Redirect("HRS.aspx");
            }

            // if its not checked binary 1 is attached
            else
            {
                CloudQueueMessage message = new CloudQueueMessage(Price.Text+"|1");
                cloudQueue.AddMessageAsync(message);
                Response.Redirect("PS.aspx");
            }
      
        }
    }
}