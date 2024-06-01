using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionHistory_Data
{
    public class TransactionHistory : TableEntity
    {
        public TransactionHistory(string userEmail, string transactionId)
        {
            this.PartitionKey = userEmail;
            this.RowKey = transactionId;
        }

        public TransactionHistory() { }

        public string CryptoSymbol { get; set; }
        public DateTime TransactionDate { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double TotalValue { get; set; }
        public bool IsPurchase { get; set; }
        public bool Buy { get; set; }
        public bool Sell { get; set; }
    }
}
