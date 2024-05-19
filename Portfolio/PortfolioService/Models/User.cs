using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace PortfolioService.Models
{
    public class User : TableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }



        public User(string id, string name, string email)
        {
            PartitionKey = "Student";
            RowKey = id;
            Name = name;
            Email = email;
        }



    }

}