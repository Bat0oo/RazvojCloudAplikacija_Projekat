using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Data
{
    public class UserData : TableEntity
    {
        //public UserData(string email, string f)
        //{
        //    this.PartitionKey = "User";
        //    this.RowKey = email;
        //    FirstName = FirstName;
        //}

        public UserData() { }

        public UserData(string firstName, string lastName, string email, string address, string city, string country, string phoneNumber, string password, string profilePicture)
        {
            this.PartitionKey = "User";
            this.RowKey = email;
            Email = email;
            FirstName = firstName;
            LastName = lastName;           
            Address = address;
            City = city;
            Country = country;
            PhoneNumber = phoneNumber;
            Password = password;
            ProfilePicture = profilePicture;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ProfilePicture { get; set; }
    }

}
