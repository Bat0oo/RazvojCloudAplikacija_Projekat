using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using Polly;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private ICheckServiceStatus servicePortfolioProxy;
        private ICheckServiceStatus serviceNotificationProxy;
        private StatusTableEntry ste;
        private HealthMonitoringService hms;
        
        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

            try
            {
                this.RunWithRetryAsync(this.cancellationTokenSource.Token).Wait();
            }

            finally
            {
                this.runCompleteEvent.Set();
            }
        }
        private async Task RunWithRetryAsync(CancellationToken token)
        {
            
            var retryPolicy = Policy
                .Handle<CommunicationException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: 3, 
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Trace.WriteLine($"Retry {retryCount} encountered a {exception.GetType().Name}. Waiting {timeSpan} before next retry. Exception: {exception.Message}");
                    });

            
            await retryPolicy.ExecuteAsync(async () =>
            {
                await RunAsync(token);
            });
        }

        private async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    EnsureConnection();
                    
                    await MonitorWebRoleStatusAsync(token);
                }
                catch (CommunicationException ex)
                {
                    Trace.WriteLine($"CommunicationException: {ex.Message}");
                    throw; 
                }
                catch (TimeoutException ex)
                {
                    Trace.WriteLine($"TimeoutException: {ex.Message}");
                    throw; 
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Unexpected exception: {ex.Message}");
                    throw; 
                }

                
            }
        }

        private async Task MonitorWebRoleStatusAsync(CancellationToken token)
        {
            

            bool servicePortfolioAvailable = servicePortfolioProxy.CheckServiceStatus();
            if (servicePortfolioAvailable)
            {
                Trace.WriteLine("PORTFOLIO is UP!! :)");
               // ste = new StatusTableEntry("PortfolioStatus","OK");
               // hms.InsertPortfolioStatus(ste);
            }
            bool notificationServicaAvailable = serviceNotificationProxy.CheckServiceStatus();
            if (notificationServicaAvailable)
            {
                Trace.WriteLine("NOTIFICATION IS UP :)");
                //ste = new StatusTableEntry("NotificationStatus", "OK");
               //hms.InsertNotificationStatus(ste);
            }


            await Task.Delay(5000);
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            hms = new HealthMonitoringService();

            ConnecToPortfolio();
            ConnecToNotificationService();

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private void EnsureConnection()
        {
            if (servicePortfolioProxy == null)
            {
                ConnecToPortfolio();
            }            
            try
            {
                servicePortfolioProxy.CheckServiceStatus(); 
            }
            
            catch (CommunicationException ex)
            {
                Trace.WriteLine("PORTFOLIO is down (CommunicationException). Reconnecting...");
                //ste = new StatusTableEntry("PortfolioStatus", "NOT_OK");
               // hms.InsertPortfolioStatus(ste);
                ConnecToPortfolio();
            }
            if (serviceNotificationProxy == null)
            {
                ConnecToNotificationService();
            }
            try
            {
                serviceNotificationProxy.CheckServiceStatus();
            }
            catch (CommunicationException ex)
            {
                Trace.WriteLine("NOTIFICATION is down (CommunicationException). Reconnecting...");
               // ste = new StatusTableEntry("NotificationStatus", "NOT_OK");
               // hms.InsertNotificationStatus(ste);
                ConnecToNotificationService();
            }

        }

        public void ConnecToPortfolio()
        {
            var endpoint = RoleEnvironment.Roles["PortfolioService"].Instances[0].InstanceEndpoints["HealthCheck"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/Service");
            var binding = new NetTcpBinding();
            
            ChannelFactory<ICheckServiceStatus> factory = new ChannelFactory<ICheckServiceStatus>(binding, address);
            servicePortfolioProxy = factory.CreateChannel();

        }

        public void ConnecToNotificationService()
        {
            var binding = new NetTcpBinding();
            var endpoint = RoleEnvironment.Roles["NotificationService"].Instances[0].InstanceEndpoints["HealthCheck"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/Service");
            ChannelFactory<ICheckServiceStatus> factory = new ChannelFactory<ICheckServiceStatus>(binding, address);
            serviceNotificationProxy = factory.CreateChannel(); // Initialize the second proxy

            

        }

    }
}
