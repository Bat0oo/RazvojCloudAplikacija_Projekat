using Crypto_Data;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using User_Data;

namespace PortfolioService
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            UserDataRepository udr = new UserDataRepository();
            CryptoDataRepository cdr = new CryptoDataRepository();

            return base.OnStart();
        }
    }
}
