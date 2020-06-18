using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{

    // Customers and Admins can upload their own photos and add a title and decription (metadata).
    // Photos need to be inside the project's webrole to be uploaded (taken from the code from the Tutorial)
    public partial class PhotoUpload : System.Web.UI.Page
    {
        private string role;
        private CloudBlobContainer cloudBlobContainer;
        protected void Page_Load(object sender, EventArgs e)
        {

            CheckCredentials();

            // Blob Client
            CloudBlobClient cloudBlobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();

            // Getting a container reference
            cloudBlobContainer = cloudBlobClient.GetContainerReference("lab4");
        }

        private void CheckCredentials()
        {
            if (User.Identity.GetUserId() == null)
            {
                Response.Redirect("/Account/Login");
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

        protected void OnUploadClick(object sender, EventArgs e)
        {
            string fileName = Server.MapPath(FileUpload.FileName);

            // create Blob
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(FileUpload.FileName);

            using (System.IO.Stream file = System.IO.File.OpenRead(fileName))
            {
                // Setting the metadata
                cloudBlockBlob.Metadata.Add("title", Title.Text);
                cloudBlockBlob.Metadata.Add("description", Description.Text);
                cloudBlockBlob.Metadata.Add("owner", User.Identity.Name);
                cloudBlockBlob.Metadata.Add("likes", "0");
                cloudBlockBlob.Metadata.Add("dislikes", "0");
                cloudBlockBlob.Metadata.Add("views", "0");
                cloudBlockBlob.UploadFromStream(file);
                cloudBlockBlob.SetMetadata();
            }
        }
    }
}