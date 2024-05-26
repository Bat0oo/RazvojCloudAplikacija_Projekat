using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto_Data
{
    public class Crypto : TableEntity
    {
        public Crypto(string symbol)
        {
            this.PartitionKey = "Crypto";
            this.RowKey = symbol;
        }

        public Crypto() { }

        public string UserEmail { get; set; }
        public string Name { get; set; }
        public double CurrentPrice { get; set; }
        public string Symbol { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
