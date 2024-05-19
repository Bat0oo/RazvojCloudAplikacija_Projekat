using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Linq;
using System;



namespace User_Data
{
    public class UserDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public UserDataRepository()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("UserTableTemp");
            _table.CreateIfNotExists();
        }

        public IQueryable<UserData> RetrieveAllUsers()
        {
            try
            {
                var results = from g in _table.CreateQuery<UserData>()
                              where g.PartitionKey == "User"
                              select g;
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during query: {ex.Message}");
                throw;
            }
        }


        public void AddUser(UserData newUser)
        {
            try
            {
                TableOperation insertOperation = TableOperation.Insert(newUser);
                TableResult result = _table.Execute(insertOperation);

                if (result.HttpStatusCode < 200 || result.HttpStatusCode >= 300)
                {
                    throw new InvalidOperationException("Failed to insert user into Azure Table Storage.");
                }

                Console.WriteLine($"Insert operation HTTP status code: {result.HttpStatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during insert operation: {ex.Message}");
                throw;
            }
        }


        public bool Exists(string email)
        {
            return RetrieveAllUsers().Where(s => s.RowKey == email).FirstOrDefault() != null;
        }

        public void RemoveUser(string email)
        {
            UserData user = RetrieveAllUsers().Where(s => s.RowKey == email).FirstOrDefault();

            if (user != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(user);
                _table.ExecuteAsync(deleteOperation).Wait();
            }
        }

        public UserData GetUser(string email)
        {
            return RetrieveAllUsers().Where(p => p.RowKey == email).FirstOrDefault();
        }

        public void UpdateUser(UserData user)
        {
            TableOperation updateOperation = TableOperation.Replace(user);
            _table.ExecuteAsync(updateOperation).Wait();
        }
    }
}
