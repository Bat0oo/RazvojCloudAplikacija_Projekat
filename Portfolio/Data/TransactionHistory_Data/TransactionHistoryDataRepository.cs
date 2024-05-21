using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace TransactionHistory_Data
{
    public class TransactionHistoryRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public TransactionHistoryRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("TransactionHistoryTable");
            _table.CreateIfNotExists();
        }

        public IQueryable<TransactionHistory> RetrieveAllTransactions()
        {
            var results = from g in _table.CreateQuery<TransactionHistory>()
                          select g;
            return results;
        }

        public void AddTransaction(TransactionHistory newTransaction)
        {
            TableOperation insertOperation = TableOperation.Insert(newTransaction);
            _table.Execute(insertOperation);
        }

        public TransactionHistory GetTransaction(string userEmail, string transactionId)
        {
            var results = from g in _table.CreateQuery<TransactionHistory>()
                          where g.PartitionKey == userEmail && g.RowKey == transactionId
                          select g;
            return results.FirstOrDefault();
        }
    }
}
