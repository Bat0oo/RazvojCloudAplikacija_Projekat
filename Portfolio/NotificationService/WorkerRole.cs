using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Alarm_Data;
using Contracts;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private ServiceHost serviceHost;
        private string internalEndpointName = "HealthCheck";
        private CloudTable alarmTable;
        private CloudTable alarmLogTable;
        private CloudQueue alarmsDoneQueue;
        private static readonly HttpClient httpClient = new HttpClient();
        private EmailSender emailSender = new EmailSender();

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Trace.TraceError($"Exception: {ex}");
                }
                throw; // Re-throw the exception after logging
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 12;

            InitializeStorage();
            InitializeServiceHost();

            bool result = base.OnStart();
            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();
            Trace.TraceInformation("NotificationService has stopped");
        }

        private void InitializeStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            alarmTable = tableClient.GetTableReference("AlarmTable");
            alarmTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            alarmLogTable = tableClient.GetTableReference("AlarmLog");
            alarmLogTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var queueClient = storageAccount.CreateCloudQueueClient();
            alarmsDoneQueue = queueClient.GetQueueReference("alarmsdone");
            alarmsDoneQueue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        private void InitializeServiceHost()
        {
            try
            {
                var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[internalEndpointName];
                var endpointAddress = $"net.tcp://{endpoint.IPEndpoint}/Service";
                serviceHost = new ServiceHost(typeof(ReportStatus), new Uri(endpointAddress));
                NetTcpBinding binding = new NetTcpBinding();
                serviceHost.AddServiceEndpoint(typeof(ICheckServiceStatus), binding, new Uri(endpointAddress));
                serviceHost.Open();
                Trace.TraceInformation("NotificationService Open.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"ServiceHost initialization failed: {ex}");
                serviceHost?.Abort();
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.TraceInformation("Working");

                    await ProcessAlarmsAsync();

                    CloudQueueMessage message = await alarmsDoneQueue.GetMessageAsync();
                    if (message == null)
                    {
                        Trace.TraceInformation("No messages in queue.");
                        await Task.Delay(10000);
                        continue;
                    }

                    if (await TryAcquireLeaseAsync(message))
                    {
                        Trace.TraceInformation($"Message received: {message.AsString}");

                        List<string> emails = new List<string>(); // Fetch or process emails list
                        if (emails.Count != 0)
                        {
                            string subject = "New comment on subscribed topic";
                            StringBuilder bodyBuilder = new StringBuilder();
                            foreach (var email in emails)
                            {
                                bodyBuilder.AppendLine(email);
                            }
                            bodyBuilder.AppendLine();
                            string body = bodyBuilder.ToString().TrimEnd();
                            body = body.Replace(Environment.NewLine, "<br/>");
                            await emailSender.SendEmailAsync("drs.productmanagement@gmail.com", subject, body);
                        }

                        Trace.TraceInformation($"Notification group sent: |Date: {DateTime.Now}| Alarm: |Emails sent: {emails.Count}|");

                        await alarmsDoneQueue.DeleteMessageAsync(message);
                    }

                    await Task.Delay(5000);
                }
                catch (StorageException ex)
                {
                    Trace.TraceError($"StorageException: {ex.Message} - {ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Exception: {ex.Message} - {ex.StackTrace}");
                }
            }
        }

        private async Task<bool> TryAcquireLeaseAsync(CloudQueueMessage message)
        {
            try
            {
                TimeSpan leaseTime = TimeSpan.FromSeconds(30);
                await alarmsDoneQueue.UpdateMessageAsync(message, leaseTime, MessageUpdateFields.Visibility);

                // Additional lease handling logic if needed
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to acquire lease: {ex.Message} - {ex.StackTrace}");
                return false;
            }
        }

        private async Task ProcessAlarmsAsync()
        {
            var query = new TableQuery<Alarm>().Where(TableQuery.GenerateFilterConditionForBool("IsTriggered", QueryComparisons.Equal, false)).Take(20);
            var alarms = await alarmTable.ExecuteQuerySegmentedAsync(query, null);

            foreach (var alarm in alarms)
            {
                bool shouldNotify = false;
                double currentPrice = await GetCurrentPriceAsync(alarm.CryptoSymbol);

                if (alarm.AboveOrBelow == "above" && currentPrice >= alarm.TargetPrice)
                {
                    shouldNotify = true;
                }
                else if (alarm.AboveOrBelow == "below" && currentPrice <= alarm.TargetPrice)
                {
                    shouldNotify = true;
                }

                if (shouldNotify)
                {
                    await SendEmailNotificationAsync(alarm);
                    await LogAlarmNotificationAsync(alarm);
                    alarm.IsTriggered = true;
                    await alarmTable.ExecuteAsync(TableOperation.Replace(alarm));
                }
            }
        }

        private async Task<double> GetCurrentPriceAsync(string cryptoSymbol)
        {
            string url = $"https://api.coinbase.com/v2/exchange-rates?currency={cryptoSymbol}";
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseData);
                double priceInUsd = json["data"]["rates"]["USD"].Value<double>();
                return priceInUsd;
            }
            return 0.0;
        }

        private async Task SendEmailNotificationAsync(Alarm alarm)
        {
            string subject = $"Alert: {alarm.CryptoSymbol} has reached your target price!";
            string body = $"The price of {alarm.CryptoSymbol} has {alarm.AboveOrBelow} your target price of {alarm.TargetPrice}.";
            await emailSender.SendEmailAsync(alarm.UserEmail, subject, body);

            await alarmsDoneQueue.AddMessageAsync(new CloudQueueMessage(alarm.RowKey));
        }

        private async Task LogAlarmNotificationAsync(Alarm alarm)
        {
            var log = new AlarmLog
            {
                PartitionKey = "AlarmLogPartition",
                RowKey = Guid.NewGuid().ToString(),
                AlarmId = alarm.RowKey,
                NotificationTime = DateTime.UtcNow,
                EmailsSent = 1
            };

            var insertOperation = TableOperation.Insert(log);
            await alarmLogTable.ExecuteAsync(insertOperation);
        }
    }
}
