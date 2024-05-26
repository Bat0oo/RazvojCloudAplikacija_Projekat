    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.Azure;
    using System.Linq;
    using System;
    using Microsoft.WindowsAzure.Storage.Blob;

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

                InitBlobs();
            }

            public void InitBlobs()
            {
                try
                {
                    Console.WriteLine("Initializing blob storage...");

                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("userimages");

                    Console.WriteLine("Creating container if not exists...");
                    container.CreateIfNotExists();

                    var permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    container.SetPermissions(permissions);

                    Console.WriteLine("Blob container initialized successfully.");
                }
                catch (StorageException ex)
                {
                    Console.WriteLine($"StorageException: {ex.Message}");
                    Console.WriteLine($"Request Information: {ex.RequestInformation}");
                    if (ex.RequestInformation.ExtendedErrorInformation != null)
                    {
                        Console.WriteLine($"Error Code: {ex.RequestInformation.ExtendedErrorInformation.ErrorCode}");
                        Console.WriteLine($"Error Message: {ex.RequestInformation.ExtendedErrorInformation.ErrorMessage}");
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during blob initialization: {ex.Message}");
                    throw;
                }
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

            public UserData GetUserByEmail(string email)
            {
                try
                {
                    TableOperation retrieveOperation = TableOperation.Retrieve<UserData>("User", email);
                    TableResult result = _table.Execute(retrieveOperation);
                    return result.Result as UserData;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during get user by email: {ex.Message}");
                    throw;
                }
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
