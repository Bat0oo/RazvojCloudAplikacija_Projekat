using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Contracts;

namespace PortfolioService.Models
{
    public class ReportStatus : ICheckServiceStatus
    {
        public bool CheckServiceStatus()
        {
            return true;
        }
    }
}