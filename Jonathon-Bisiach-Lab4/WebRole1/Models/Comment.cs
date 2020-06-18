using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1.Models
{
    public class Comment : TableEntity
    {
        // This class connects with the Azure Table to create table items.
        public String imageName
        {
            get; set;
        }
        public String commentText
        {
            get; set;
        }
        public String userName
        {
            get; set;
        }

        public Comment(string imageName, string commentText, string userName)
        {
            this.imageName = imageName;
            this.commentText = commentText;
            this.userName = userName;
            PartitionKey = "Partiton";
            RowKey = imageName + userName;
        }
        public Comment()
        {

        }
    }
}