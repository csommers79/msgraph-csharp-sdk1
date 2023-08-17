using Microsoft.Extensions.Configuration;
using DomainObjects.Models;
using DomainObjects.Services;
using DomainObjects.Logging;

namespace ReadUserDriveInfo
{
    internal class Program
    {       

        static async Task Main(string[] args)
        {                        
            try
            {

                IConfiguration config = new ConfigurationBuilder()
                                         .AddJsonFile("appsettings.json")
                                         .AddEnvironmentVariables()
                                         .Build();

                // Get values from the config given their key and their target type.
                var AppId = config["AppId"];
                var TenantId = config["TenantId"];
                var AppSecret = config["AppSecret"];                
                var EmailAddresses = config["EmailAddresses"];
                AppConfig configData = new AppConfig
                {
                    AppId = AppId,
                    AppSecret = AppSecret,
                    TenantId = TenantId,                  
                    EmailAddress = EmailAddresses

                };
                await MSGraphApiService.GetInstance(configData).ReadOneDrive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);                
            }
        }
    }
}