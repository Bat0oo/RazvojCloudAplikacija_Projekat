using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Linq;
using System;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;

namespace Alarm_Data
{
    public class AlarmDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        private CloudTable _alarmLogTable;
        private CloudQueue _alarmsDoneQueue;

        public AlarmDataRepository()
        {
            string connectionString = CloudConfigurationManager.GetSetting("DataConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string for Azure Storage is not set.");
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("AlarmTable");
            _table.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            _alarmLogTable = tableClient.GetTableReference("AlarmLog");
            _alarmLogTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            CloudQueueClient queueClient = _storageAccount.CreateCloudQueueClient();
            _alarmsDoneQueue = queueClient.GetQueueReference("alarmsdone");
            _alarmsDoneQueue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var permissions = new QueuePermissions();
            permissions.SharedAccessPolicies.Add("mypolicy", new SharedAccessQueuePolicy
            {
                Permissions = SharedAccessQueuePermissions.Add |
                              SharedAccessQueuePermissions.Read |
                              SharedAccessQueuePermissions.ProcessMessages |
                              SharedAccessQueuePermissions.Update,
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddYears(1)
            });
            _alarmsDoneQueue.SetPermissions(permissions);
        }

        public static CloudQueue GetQueueReference(String queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
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

        public Alarm GetAlarmById(string id)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Alarm>("Alarm", id);
            TableResult result = _table.Execute(retrieveOperation);
            return result.Result as Alarm;
        }

        public async Task AddToAlarmsDoneQueueAsync(string alarmId)
        {
            CloudQueueMessage message = new CloudQueueMessage(alarmId);
            await _alarmsDoneQueue.AddMessageAsync(message);
        }

    }
}
