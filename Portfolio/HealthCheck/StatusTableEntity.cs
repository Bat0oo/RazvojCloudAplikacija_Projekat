using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class StatusTableEntity:TableEntity
    {

        public string Status { get; set; }
    }
}
