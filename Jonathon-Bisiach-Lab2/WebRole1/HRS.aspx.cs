﻿using Microsoft.WindowsAzure.Storage;
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
    public partial class HRS : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Offer(object sender, EventArgs e)
        {

            string roomType = RoomType.SelectedValue;
            string primaryGuest = PrimaryGuest.Text;
            string nbrTravellers = Travellers.Text;
            string nbrSeniors = Seniors.Text;
            string nbrNights = Nights.Text;

            // All data in one variable; separated by "|" character

            string send = roomType + "|" + primaryGuest + "|" + nbrTravellers + "|" + nbrSeniors + "|" + nbrNights;

            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("hoteloffer");
            CloudQueueMessage message = new CloudQueueMessage(send);
            cloudQueue.AddMessageAsync(message);

            CloudQueue returnQueue = client.GetQueueReference("return-hoteloffer");
            CloudQueueMessage receive = null;

            // continues until message received
            while (receive == null)
            {
                receive = returnQueue.GetMessage();
                Task.Delay(1000);
            }

            returnQueue.DeleteMessage(receive);
            
            // send the price to the label
            Price.Text = receive.AsString;
            goOn.Visible = true;

        }
        public void Continue(object sender, EventArgs e)
        {
            CloudQueueClient client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudQueueClient();
            CloudQueue cloudQueue = client.GetQueueReference("payment");
            CloudQueueMessage message = new CloudQueueMessage(Price.Text + "|1");
            cloudQueue.AddMessageAsync(message);

            Response.Redirect("PS.aspx");

            // when the 'Continue' button is pressed the price is sent to the payment queue and the page is redirected to the payment page.
        }
    }
}