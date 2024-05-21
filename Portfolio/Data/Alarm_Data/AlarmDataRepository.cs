using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Alarm_Data
{
    public class AlarmDataRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public AlarmDataRepository()
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
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
            _table.Execute(insertOperation);
        }

        public Alarm GetAlarm(string userEmail, string alarmId)
        {
            var results = from g in _table.CreateQuery<Alarm>()
                          where g.PartitionKey == userEmail && g.RowKey == alarmId
                          select g;
            return results.FirstOrDefault();
        }

        public void UpdateAlarm(Alarm alarm)
        {
            TableOperation updateOperation = TableOperation.Replace(alarm);
            _table.Execute(updateOperation);
        }

        public void RemoveAlarm(string userEmail, string alarmId)
        {
            Alarm alarm = GetAlarm(userEmail, alarmId);

            if (alarm != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(alarm);
                _table.Execute(deleteOperation);
            }
        }
    }

}
