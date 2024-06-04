using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EmailConfiguration
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            string success = "";
            string email = "";
            ConfigureEmail eConfig = new ConfigureEmail();
            eConfig.ConnectToHealthMonitoringService();            
            while(true)
            {
                Console.WriteLine("Unesite email na koji zelite da saljete upozorenje: ");
                email = Console.ReadLine();
                success = eConfig.servicePortfolioProxy.ConfigureEmail(email);
                Console.WriteLine(success + "\n");
            }    

            

            
        }

        
    }

}
