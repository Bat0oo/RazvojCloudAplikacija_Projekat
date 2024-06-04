using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class ErrorMessageEmail : TableEntity
    {
        public ErrorMessageEmail(string email)
        {
            this.PartitionKey = "Email";
            this.RowKey = email;
            
        }
        public ErrorMessageEmail()
        {

        }
    }
}
