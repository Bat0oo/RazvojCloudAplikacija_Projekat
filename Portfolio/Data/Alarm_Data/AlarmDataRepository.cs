using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Linq;
using System;

namespace Alarm_Data
{
    public class AlarmDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public AlarmDataRepository()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("AlarmTable");
            _table.CreateIfNotExists();

        }

        public IQueryable<Alarm> RetrieveAllAlarms()
        {
            var results = from g in _table.CreateQuery<Alarm>()
                          select g;
            return results;
        }

        public void AddAlarm(Alarm newAlarm)
        {
            TableOperation insertOperation = TableOperation.Insert(newAlarm);
            TableResult result = _table.Execute(insertOperation);
        }

        public Alarm GetAlarm(string userEmail)
        {
            var results = from g in _table.CreateQuery<Alarm>()
                          where g.PartitionKey == userEmail
                          select g;
            return results.FirstOrDefault();
        }

        public IQueryable<Alarm> GetAlarmsByUserEmail(string userEmail)
        {
            var results = from g in _table.CreateQuery<Alarm>()
                          where g.UserEmail == userEmail
                          select g;
            return results;
        }

        public void UpdateAlarm(Alarm alarm)
        {
            TableOperation updateOperation = TableOperation.Replace(alarm);
            _table.Execute(updateOperation);
        }

        public void RemoveAlarm(string userEmail, string alarmId)
        {
            Alarm alarm = GetAlarm(userEmail);

            if (alarm != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(alarm);
                _table.Execute(deleteOperation);
            }
        }
    }
}
