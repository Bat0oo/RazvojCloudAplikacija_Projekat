using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Crypto_Data
{
    public class CryptoDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public CryptoDataRepository()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("CryptoTable");
            _table.CreateIfNotExists();
        }

        public IEnumerable<Crypto> RetrieveAllCryptos(string email)
        {
            var results = from g in _table.CreateQuery<Crypto>()
                          where g.PartitionKey == "Crypto"
                          where g.UserEmail == email
                          select g;
            return results;
        }

        public void AddCrypto(Crypto newCrypto)
        {
            try
            {
                TableOperation insertOperation = TableOperation.Insert(newCrypto);
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

        /*
        public Crypto GetCrypto(string symbol)
        {
            return RetrieveAllCryptos().Where(p => p.RowKey == symbol).FirstOrDefault();
        }
        */

        public Crypto GetCryptoBySymbol(string userEmail, string symbol)
        {
            var result = (from g in _table.CreateQuery<Crypto>()
                          where g.PartitionKey == "Crypto" && g.UserEmail == userEmail && g.RowKey == symbol
                          select g).FirstOrDefault();
            return result;
        }


        public void UpdateCrypto(Crypto crypto)
        {
            TableOperation updateOperation = TableOperation.Replace(crypto);
            _table.Execute(updateOperation);
        }

        public void DeleteCrypto(string userEmail, string symbol)
        {
            try
            {
                var cryptoToDelete = GetCryptoBySymbol(userEmail, symbol);
                if (cryptoToDelete != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(cryptoToDelete);
                    _table.Execute(deleteOperation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during delete operation: {ex.Message}");
                throw;
            }
        }


    }
}
