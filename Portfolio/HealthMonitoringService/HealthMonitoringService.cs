using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace HealthMonitoringService
{
    public class HealthMonitoringService
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table1;
        private CloudTable _table2;

        public HealthMonitoringService()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table1 = tableClient.GetTableReference("HealthMonitoringServicePortfolio");
            _table2 = tableClient.GetTableReference("HealthMonitoringServiceNotification");
            _table1.CreateIfNotExists();
            _table2.CreateIfNotExists();
        }

        public void InsertPortfolioStatus(StatusTableEntry status)
        {

            TableOperation insertOperation = TableOperation.Insert(status);
            TableResult result = _table1.Execute(insertOperation);




        }
        public void InsertNotificationStatus(StatusTableEntry status)
        {

            TableOperation insertOperation = TableOperation.Insert(status);
            TableResult result = _table2.Execute(insertOperation);

        }

    }
}
