using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EmailConfiguration
{
    public class ConfigureEmail
    {
        public ISetEmail servicePortfolioProxy;
        
        public void ConnectToHealthMonitoringService()
        {

            var address = new EndpointAddress("net.tcp://127.0.0.1:10100/Email");
            var binding = new NetTcpBinding();

            ChannelFactory<ISetEmail> factory = new ChannelFactory<ISetEmail>(binding, address);
            servicePortfolioProxy = factory.CreateChannel();
        }
    }
}

