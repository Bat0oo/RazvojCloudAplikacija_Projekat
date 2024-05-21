using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class StatusTableEntry : TableEntity
    {
        public StatusTableEntry(string particionName, string status)
        {
            this.PartitionKey = particionName;
            this.RowKey = DateTime.Now.ToString();
            Status = status;
            
        }
        
        public string Status { get; set; }
        
    }
}
