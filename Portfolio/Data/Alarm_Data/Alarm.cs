using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarm_Data
{
    public class Alarm : TableEntity
    {
        public Alarm(string alarmId)
        {
            this.PartitionKey = "AlarmPartition";
            this.RowKey = alarmId;
        }

        public Alarm() { }

        public string UserEmail { get; set; }
        public string CryptoSymbol { get; set; }
        public double TargetPrice { get; set; }
        public string AboveOrBelow { get; set; }
        public bool IsTriggered { get; set; }

        public static string GeneratePartitionKey()
        {
            return Guid.NewGuid().ToString();
        }
    }

    public class AlarmLog : TableEntity
    {
        public AlarmLog(string logId)
        {
            this.PartitionKey = "AlarmLogPartition";
            this.RowKey = logId;
        }

        public AlarmLog() { }

        public string AlarmId { get; set; }
        public DateTime NotificationTime { get; set; }
        public int EmailsSent { get; set; }

        public static string GeneratePartitionKey()
        {
            return Guid.NewGuid().ToString();
        }
    }

}
