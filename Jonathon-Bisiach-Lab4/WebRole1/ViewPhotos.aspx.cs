using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{
    public partial class ViewPhotos : System.Web.UI.Page
    {
        // Visitors can view any photo upload by a customer or admin. 
        // Visitors must download the photos to view.
        // Customers can view pictures, like , dislike and comment photos uploaded by another customer, delete their own photos.

        private string role;
        private CloudBlobContainer cloudBlobContainer;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckCredentials();

            // Blob client
            CloudBlobClient cloudBlobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();

            // Getting a container reference
            cloudBlobContainer = cloudBlobClient.GetContainerReference("lab4");

            BlobContinuationToken blobContinuationToken = null;
            var results = cloudBlobContainer.ListBlobsSegmented(null, blobContinuationToken);
            blobContinuationToken = results.ContinuationToken;

            foreach (CloudBlockBlob cbb in results.Results)
            {
                cbb.FetchAttributes(); 
                // get metadata

                TableRow tableRow = new TableRow();

                TableCell title = new TableCell();
                title.Text = cbb.Metadata["title"];
                tableRow.Cells.Add(title);

                TableCell description = new TableCell();
                description.Text = cbb.Metadata["description"];
                tableRow.Cells.Add(description);

                TableCell owner = new TableCell();
                owner.Text = cbb.Metadata["owner"];
                tableRow.Cells.Add(owner);

                TableCell download = new TableCell();
                Button downloadButton = new Button();
                downloadButton.Text = "Download photo to see it";
                downloadButton.Click += ButtonDownload;
                downloadButton.CommandArgument = cbb.Uri.ToString();
                download.Controls.Add(downloadButton);
                tableRow.Cells.Add(download);

                if (User.Identity.Name == cbb.Metadata["owner"] || role == "Administrator")
                {
                    TableCell delete = new TableCell();
                    Button deleteButton = new Button();
                    deleteButton.Text = "Delete photo!";
                    deleteButton.Click += ButtonDelete;
                    deleteButton.CommandArgument = cbb.Uri.ToString();
                    delete.Controls.Add(deleteButton);
                    tableRow.Cells.Add(delete);
                }
                else if (User.Identity.GetUserId() != null)
                {
                    TableCell like = new TableCell();
                    Button likeButton = new Button();
                    likeButton.Text = "Like photo!";
                    likeButton.Click += ButtonLike;
                    likeButton.CommandArgument = cbb.Uri.ToString();
                    like.Controls.Add(likeButton);
                    tableRow.Cells.Add(like);

                    TableCell dislike = new TableCell();
                    Button dislikeButton = new Button();
                    dislikeButton.Text = "Dislike photo!";
                    dislikeButton.Click += ButtonDislike;
                    dislikeButton.CommandArgument = cbb.Uri.ToString();
                    dislike.Controls.Add(dislikeButton);
                    tableRow.Cells.Add(dislike);

                    TableCell commentText = new TableCell();
                    TextBox textBox = new TextBox();
                    commentText.Controls.Add(textBox);
                    tableRow.Cells.Add(commentText);

                    TableCell comment = new TableCell();
                    Button commentButton = new Button();
                    commentButton.Text = "Comment on the photo!";
                    commentButton.Click += ButtonComment;
                    commentButton.CommandArgument = cbb.Uri.ToString();
                    comment.Controls.Add(commentButton);
                    tableRow.Cells.Add(comment);
                }

                table.Rows.Add(tableRow);
            }
        }

        private void CheckCredentials()
        {
            if (User.Identity.GetUserId() == null)
            {

            }
            else
            {
                role = getUserRole();
            }
        }

        protected string getUserRole()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (User.Identity.GetUserId() == null) return null;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var myroles = roleManager.Roles.ToList();
            foreach (IdentityRole arole in myroles)
            {
                string roleName = arole.Name;
                if (User.IsInRole(roleName))
                    return arole.Name;
            }
            return null;
        }

        protected void ButtonDownload(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string value = button.CommandArgument;  
            // Value is the entire URL of the file.
            string[] split = value.Split('/');
            string fileName = split[split.Length - 1]; 
            // By splitting only the file name is obtained  

            try
            {

                string targetFile = "C:/Users/user/Downloads/" + fileName;

                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);
                blob.DownloadToFileAsync(targetFile, System.IO.FileMode.Create);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected void ButtonDelete(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string value = button.CommandArgument;
            string[] split = value.Split('/');
            string fileName = split[split.Length - 1];

            try
            {
                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);
                blob.Delete();
                Debug.WriteLine("Picture is deleted!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected void ButtonLike(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string value = button.CommandArgument;
            string[] split = value.Split('/');
            string fileName = split[split.Length - 1];

            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);
            blob.FetchAttributes();

            int likes = Int32.Parse(blob.Metadata["likes"]);   //store likes in the integer
            blob.Metadata["likes"] = (likes + 1).ToString();   // increase when we type the like button

            blob.SetMetadata();  // update to storage
            Debug.WriteLine(blob.Metadata["likes"]);      // The likes will be shown in the output in visual studio
        }

        protected void ButtonDislike(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string value = button.CommandArgument;
            string[] split = value.Split('/');
            string fileName = split[split.Length - 1];

            CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);
            blob.FetchAttributes();

            int dislikes = Int32.Parse(blob.Metadata["dislikes"]);
            blob.Metadata["dislikes"] = (dislikes + 1).ToString();

            blob.SetMetadata();
            Debug.WriteLine(blob.Metadata["dislikes"]);
        }

        protected void ButtonComment(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string value = button.CommandArgument;
            string[] split = value.Split('/');
            string fileName = split[split.Length - 1];

            TableCell parentButtonCell = (TableCell)button.Parent;
            TableRow parentRow = (TableRow)parentButtonCell.Parent;
            TextBox commentText = (TextBox)parentRow.Cells[6].Controls[0];

            Comment comment = new Comment(fileName, commentText.Text, User.Identity.Name);

            CloudTableClient tableClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudTableClient();
            CloudTable commentsTable = tableClient.GetTableReference("comments");

            TableOperation insert = TableOperation.Insert(comment);
            commentsTable.Execute(insert);
        }
    }
}