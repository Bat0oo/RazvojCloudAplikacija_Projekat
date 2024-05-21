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
        public Alarm(string userEmail, string alarmId)
        {
            this.PartitionKey = userEmail;
            this.RowKey = alarmId;
        }

        public Alarm() { }

        public string CryptoSymbol { get; set; }
        public double TargetPrice { get; set; }
        public bool IsAbove { get; set; }
        public bool IsBelow { get; set; }
        public bool IsTriggered { get; set; }
    }
}
