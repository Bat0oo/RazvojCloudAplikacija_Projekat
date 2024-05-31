using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using User_Data;
using PortfolioService.Models;
using Contracts;
using System.Threading;
using NotificationService;

namespace PortfolioService
{
    public class WebRole : RoleEntryPoint
    {
        private string internalEndpointName = "HealthCheck";

       

        public override bool OnStart()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HealthCheck"];
            var endpointAddress = $"net.tcp://{endpoint.IPEndpoint}/Service";
            //string endpointAddress = "net.tcp://127.0.0.1:6000/Service";
            ServiceHost serviceHost = new ServiceHost(typeof(ReportStatus));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(ICheckServiceStatus), binding, endpointAddress);
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests.");

            


            return base.OnStart();
        }
    }
}
