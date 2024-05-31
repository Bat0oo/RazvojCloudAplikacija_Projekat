using Contracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        ServiceHost serviceHost;
        private string internalEndpointName = "HealthCheck";
        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

           
            try
            {

                var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HealthCheck"];
                var endpointAddress = $"net.tcp://{endpoint.IPEndpoint}/Service";
                
                serviceHost = new ServiceHost(typeof(ReportStatus),new Uri(endpointAddress));
                NetTcpBinding binding = new NetTcpBinding();
                serviceHost.AddServiceEndpoint(typeof(ICheckServiceStatus), binding, new Uri(endpointAddress));
                serviceHost.Open();
                Trace.TraceInformation("NotificationServiceOpen.");


            }
            catch
            {
                serviceHost.Abort();
            }




            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

           

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

       

       


        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                // Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

    }
}
