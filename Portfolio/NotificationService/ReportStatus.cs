using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace NotificationService
{
    public class ReportStatus : ICheckServiceStatus
    {
        public bool CheckServiceStatus()
        {
            return true;
        }
    }
}
