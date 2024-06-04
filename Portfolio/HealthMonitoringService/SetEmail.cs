using Contracts;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthMonitoringService
{
    public class SetEmail : ISetEmail
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public SetEmail()
        {
            ConnectToStorage();
        }

        public string ConfigureEmail(string recipient)
        {
            var existingEmail = RetrieveEmail();
            if (existingEmail.Any())
            {
                var deleteEmail = existingEmail.First();
                TableOperation tableOperation = TableOperation.Delete(deleteEmail);
                _table.Execute(tableOperation);
            }

            // Insert new email if none exists
            ErrorMessageEmail email = new ErrorMessageEmail(recipient);
            TableOperation insertOperation = TableOperation.Insert(email);
            TableResult insertResult = _table.Execute(insertOperation);


            return "uspesno";



        }

        public void ConnectToStorage()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("ErrorMessageEmail");
            _table.CreateIfNotExists();

        }

        public IEnumerable<ErrorMessageEmail> RetrieveEmail()
        {
            try
            {
                TableQuery<ErrorMessageEmail> query = new TableQuery<ErrorMessageEmail>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Email"));

                var results = _table.ExecuteQuery(query);
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during query: {ex.Message}");
                throw;
            }
        }
        public ErrorMessageEmail RetrieveFrist()
        {
            var emails = RetrieveEmail();
            ErrorMessageEmail email = emails.First();
            return email;
        }





    }
}
