using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using System.Collections;
using System.ComponentModel;
using DomainObjects.Models;
using DomainObjects.Services;
using DomainObjects.Logging;

namespace ReadUserDriveInfo
{
    internal class Program
    {       
       
        private static GraphServiceClient graphServiceClient = null;

        //static void Main(string[] args)
        //{
        //    GetUsers().GetAwaiter().GetResult();
        //}


        //async static Task GetUsers()
        //{
        //    try
        //    {
        //        var configuration = new ConfigurationBuilder()
        //                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
        //                    .AddJsonFile($"appsettings.json");

        //        var config = configuration.Build();

        //        var clientId = config["ClientId"];
        //        var clientSecret = config["AppSecret"];
        //        var tenantId = config["TenantId"];
        //        var appId = config["AppId"];
        //        var filter = $"startswith(Mail, 'kenneth')";
        //        Encoding encoding = Encoding.GetEncoding(28591);
        //        //using (StreamWriter file = new StreamWriter(_filePath, false, encoding))
        //        {
        //            string headerLine = "AccountEnabled;City;Country;OfficeLocation;Department;CompanyName;DisplayName;EmployeeId;GivenName;JobTitle;Mail;MailNickname;MobilePhone;Manager;OnPremisesSecurityIdentifier;PasswordPolicies;PostalCode;State;StreetAddress;Surname;UsageLocation;UserPrincipalName;UserType;OnPremisesDomainName;OnPremisesSamAccountName;OnPremisesSyncEnabled;OnPremisesLastSyncDateTime;userLicenses;extension_division;extension_employeeType;extension_employeeNumber";
        //            //file.WriteLine(headerLine);

        //            IConfidentialClientApplication publicClientApp = ConfidentialClientApplicationBuilder.Create(clientId)
        //                                      .WithClientSecret(clientSecret)
        //                                      //.WithAuthority(new Uri("https://login.microsoftonline.com/common"))
        //                                      .WithAuthority(new Uri("https://login.microsoftonline.com/" + tenantId))
        //                                      .Build();

        //            graphServiceClient =
        //            new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
        //            {
        //                var scopes = new[] { "https://graph.microsoft.com/.default" };

        //                var authResult = await publicClientApp.AcquireTokenForClient(scopes).ExecuteAsync();

        //                // Add the access token in the Authorization header of the API
        //                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        //            }));

        //            var employeeIDExtensionAttribute = $"extension_{appId}_company";
        //            var employeeTypeExtensionAttribute = $"extension_{appId}_employeeType";
        //            var employeeNumberExtensionAttribute = $"extension_{appId}_employeeNumber";
        //            var physicalDeliveryOfficenameExtensionAttribute = $"extension_{appId}_physicalDeliveryOfficename";
        //            var companyExtensionAttribute = $"extension_{appId}_company";
        //            var divisionExtensionAttribute = $"extension_{appId}_division";

        //            var usersPage = await graphServiceClient.Users
        //                .Request()                        
        //                .Select($"id,accountEnabled, assignedLicenses, assignedPlans, city, country, creationType, deletionTimestamp, licenseDetails, officeLocation, department, companyname, dirSyncEnabled, displayName, employeeId, facsimileTelephoneNumber, givenName, immutableId, jobTitle, lastDirSyncTime, mail, mailNickname, mobilePhone, objectId, objectType, onPremisesSecurityIdentifier, otherMails, passwordPolicies, passwordProfile, physicalDeliveryOfficeName, postalCode, preferredLanguage, provisionedPlans, provisioningErrors, proxyAddresses, refreshTokensValidFromDateTime, showInAddressList, signInNames, sipProxyAddress, state, streetAddress, surname, telephoneNumber, thumbnailPhoto, usageLocation, userIdentities, userPrincipalName, userType, OnPremisesDomainName, OnPremisesSamAccountName, OnPremisesSyncEnabled, OnPremisesLastSyncDateTime, onPremisesExtensionAttributes, extension_{appId}_employeeType, extension_{appId}_employeeNumber, extension_{appId}_physicalDeliveryOfficename, extension_{appId}_company, extension_{appId}_extensionAttribute1, extension_{appId}_division")
        //                .GetAsync();

        //            ////////////////////////////////////////////////////////////////////
        //            // PRODUCT DICTIONARY GOES HERE - PLEASE SEPARATE IN OWN FUNCTION //
        //            ////////////////////////////////////////////////////////////////////
        //            //await LoadUserLicense();
        //            //using (var context = new AzureUserLogsContext())
        //            {
        //                IConfidentialClientApplication publicClientAppForDrive = ConfidentialClientApplicationBuilder.Create(clientId)
        //                                      .WithClientSecret(clientSecret)
        //                                      .WithAuthority(new Uri("https://login.microsoftonline.com/common"))
        //                                      //.WithAuthority(new Uri("https://login.microsoftonline.com/" + tenantId))
        //                                      .Build();

        //               var graphServiceClientForDrive =
        //                new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
        //                {
        //                    var scopes = new[] { "https://graph.microsoft.com/.default" };

        //                    var authResult = await publicClientAppForDrive.AcquireTokenForClient(scopes).ExecuteAsync();

        //                    // Add the access token in the Authorization header of the API
        //                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
        //                }));
        //                foreach (var u in usersPage)
        //                {
        //                    var drive = await graphServiceClient.Users[u.Id].Drive
        //                                       .Request()
        //                                       .GetAsync();
        //                }
        //                //SaveToDb(usersPage, appId);
        //                //var userLines = GetUserLines(usersPage, appId);
        //                //Console.Write(userLines);
        //                //await file.WriteAsync(userLines);

        //                while (usersPage.NextPageRequest != null)
        //                {
        //                    usersPage = await usersPage.NextPageRequest.GetAsync();
        //                    //SaveToDb(usersPage, appId);
        //                    //userLines = GetUserLines(usersPage, appId);
        //                    //Console.Write(userLines);
        //                    //await file.WriteAsync(userLines);
        //                }
        //                //DeleteAllDBData(context);
        //                //context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.ReadLine();
        //    }

        //    Console.WriteLine("Press any key to exit.");
        //}

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            FileLogger logger = null;
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
                var LocalTargetFolder = config["LocalTargetFolder"];
                AppConfig configData = new AppConfig
                {
                    AppId = AppId,
                    AppSecret = AppSecret,
                    TenantId = TenantId,                  
                    EmailAddress = EmailAddresses,
                    LocalTargetFolder = LocalTargetFolder

                };
                await MSGraphApiService.GetInstance(configData, logger).UploadFiles();
            }
            catch (Exception ex)
            {
                logger.Error(Newtonsoft.Json.JsonConvert.SerializeObject(ex));
            }
        }
    }
}