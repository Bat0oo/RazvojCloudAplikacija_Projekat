using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class DataHealthCheck
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table1;
        public DataHealthCheck()
        {

            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table1 = tableClient.GetTableReference("HealthMonitoringServicePortfolio");
            _table1.CreateIfNotExists();
        }
        public List<StatusTableEntity> GetData()
        {
            var result = new List<StatusTableEntity>();

            // Query for data from the last hour where status is not_ok
            var lastHour = DateTime.UtcNow.AddHours(-1);
            var lastHourFilter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, lastHour),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("Status", QueryComparisons.Equal, "NOT_OK")
            );
            var lastHourQuery = new TableQuery<StatusTableEntity>().Where(lastHourFilter);
            result.AddRange(ExecuteQuery(lastHourQuery));

            // Query for data from the last 24 hours
            var last24Hours = DateTime.UtcNow.AddHours(-24);
            var last24HoursFilter = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, last24Hours);
            var last24HoursQuery = new TableQuery<StatusTableEntity>().Where(last24HoursFilter);
            var last24HoursData = ExecuteQuery(last24HoursQuery);

            // Calculate percentage of OK statuses in the last 24 hours
            var totalEntries = last24HoursData.Count;
            var okEntries = last24HoursData.Count(e => e.Status == "OK");
            var okPercentage = totalEntries > 0 ? (double)okEntries / totalEntries * 100 : 0;
            okPercentage=Math.Round(okPercentage,2);
            // Add the percentage as a special entry to the result (you can customize this as needed)
            result.Add(new StatusTableEntity
            {
                PartitionKey = "Summary",
                RowKey = "Last 24h uptime percentage: ",
                Status = $"{okPercentage}%"
            });

            return result;
        }

        private List<StatusTableEntity> ExecuteQuery(TableQuery<StatusTableEntity> query)
        {
            var result = new List<StatusTableEntity>();
            TableContinuationToken token = null;
            do
            {
                var queryResult = _table1.ExecuteQuerySegmented(query, token);
                result.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return result;
        }
    }
}





