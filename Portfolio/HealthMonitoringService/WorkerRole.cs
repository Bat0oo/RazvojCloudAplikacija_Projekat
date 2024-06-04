using Contracts;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Polly;
using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

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
        private EmailSender eSender = new EmailSender();
        private SetEmail errorEmail;
        
        

        

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
                    await EnsureConnection();

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
                try
                {

                    ste = new StatusTableEntry("PortfolioStatus", "OK");                    
                    
                    
                    hms.InsertPortfolioStatus(ste);
                }
                catch(StorageException ex)
                {
                    if(ex.RequestInformation.HttpStatusCode == 409)
                    {
                        //Trace.WriteLine("OtherInstance already inserted into the table");
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            bool notificationServicaAvailable = serviceNotificationProxy.CheckServiceStatus();
            if (notificationServicaAvailable)
            {
                Trace.WriteLine("NOTIFICATION IS UP!! :)");
                try
                {
                    ste = new StatusTableEntry("NotificationStatus", "OK");
                    hms.InsertNotificationStatus(ste);

                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 409)
                    {
                        //Trace.WriteLine("OtherInstance already inserted into the table");
                    }
                    else
                    {
                        throw;
                    }
                }

            }


            await Task.Delay(10000);
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            hms = new HealthMonitoringService();
            errorEmail = new SetEmail();

            ConnecToPortfolio();
            ConnecToNotificationService();
            OpenSetEmailEndpoint();

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

        private async Task EnsureConnection()
        {
            if (servicePortfolioProxy == null)
            {
                ConnecToPortfolio();
                
            }
            try
            {
                servicePortfolioProxy.CheckServiceStatus();
            }

            catch (CommunicationException ce)
            {
                Trace.WriteLine("PORTFOLIO is down (CommunicationException). Reconnecting...");
                try
                {
                    ste = new StatusTableEntry("PortfolioStatus", "NOT_OK");
                    ErrorMessageEmail email = errorEmail.RetrieveFrist();

                    await SendEmailNotificationAsync("Portfolio", email.RowKey);
                    
                    
                   
                    
                    
                    hms.InsertPortfolioStatus(ste);
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 409)
                    {
                        //Trace.WriteLine("OtherInstance already inserted into the table");
                    }
                    else
                    {
                        throw;
                    }
                }


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
            catch (CommunicationException ce)
            {
                Trace.WriteLine("NOTIFICATION is down (CommunicationException). Reconnecting...");

                try
                {
                    ste = new StatusTableEntry("NotificationStatus", "NOT_OK");
                    ErrorMessageEmail email = errorEmail.RetrieveFrist();                    
                    await SendEmailNotificationAsync("Notification", email.RowKey);
                    hms.InsertNotificationStatus(ste);
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 409)
                    {
                        //Trace.WriteLine("OtherInstance already inserted into the table");
                    }
                    else
                    {
                        throw;
                    }
                }


                ConnecToNotificationService();
                await Task.Delay(5000);
            }

        }

        public void ConnecToPortfolio()
        {
            var endpoint = RoleEnvironment.Roles["PortfolioService"].Instances[0].InstanceEndpoints["HealthCheck"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/Service");
            //var address = new EndpointAddress("net.tcp://127.0.0.1:6000/Service");
            var binding = new NetTcpBinding();

            ChannelFactory<ICheckServiceStatus> factory = new ChannelFactory<ICheckServiceStatus>(binding, address);
            servicePortfolioProxy = factory.CreateChannel();

        }
        private async Task SendEmailNotificationAsync(string serviceName, string recipiant)
        {
            
            string subject = $"Critical Error";
            string body = $"{serviceName} is DOWN!";
            await eSender.SendEmailAsync(recipiant, subject, body);            
        }


        public void ConnecToNotificationService()
        {
            var binding = new NetTcpBinding();
            var endpoint = RoleEnvironment.Roles["NotificationService"].Instances[0].InstanceEndpoints["HealthCheck"];
            var address = new EndpointAddress($"net.tcp://{endpoint.IPEndpoint}/Service");
            //var address = new EndpointAddress("net.tcp://127.0.0.1:20002/Service");
            ChannelFactory<ICheckServiceStatus> factory = new ChannelFactory<ICheckServiceStatus>(binding, address);
            serviceNotificationProxy = factory.CreateChannel(); // Initialize the second proxy



        }

        
        public void OpenSetEmailEndpoint()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["SetEmail"];
            var endpointAddress = $"net.tcp://{endpoint.IPEndpoint}/Email";
            ServiceHost serviceHost = new ServiceHost(typeof(SetEmail));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(ISetEmail), binding, endpointAddress);
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests.");
        }

        

        
    }
}