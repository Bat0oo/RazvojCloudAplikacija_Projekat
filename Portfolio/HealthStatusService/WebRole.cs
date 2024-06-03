using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthStatusService
{
    public class WebRole : RoleEntryPoint
    {

        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.


            return base.OnStart();
        }



    }
}
