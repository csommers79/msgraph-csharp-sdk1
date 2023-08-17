using DomainObjects.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace DomainObjects.Services
{
    public class MSGraphApiService
    {
        private static object _lock = new object();
        private static MSGraphApiService _instance;
        private AppConfig _appConfig;
        static readonly HttpClient client = new HttpClient();
        private string EmailAdress;
        private GraphServiceClient graphServiceClient;
        private MSGraphApiService(AppConfig appConfig)
        {
            _appConfig = appConfig;            
            EmailAdress = appConfig.EmailAddress.Trim();
        }

        public static MSGraphApiService GetInstance(AppConfig appConfig)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MSGraphApiService(appConfig);
                    }
                }
            }
            return _instance;
        }

        public async Task ReadOneDrive()
        {
            try
            {
                this.graphServiceClient = GetGraphClient();
                {
                    var u = await graphServiceClient.Users[EmailAdress].Request().GetAsync();
                    Dictionary<string, List<dynamic>> keyValuePairs = new Dictionary<string, List<dynamic>>();

                    try
                    {
                       var userDrives = await graphServiceClient.Users[u.Id].Drives
                                               .Request()
                                               .GetAsync();

                        Console.WriteLine("** Available user drives **");
                        await PrintUserDrives(userDrives, u.Id);
                        while (userDrives.NextPageRequest != null)
                        {
                            userDrives = await userDrives.NextPageRequest.GetAsync();
                            await PrintUserDrives(userDrives, u.Id);
                        }   
                        

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            catch (ServiceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


       private async Task PrintUserDrives(IUserDrivesCollectionPage userDrives, string userId)
        { 
            foreach (var d in userDrives)
            {
                Console.WriteLine(string.Format("Drive Name = {0}, Drive Id = {1}", d.Name, d.Id));
                var rootFolders = await graphServiceClient.Users[userId].Drives[d.Id].Root.Children
                                                .Request()
                                                .GetAsync();
               
                Console.WriteLine("** Available folders **");
                await PrintDriveItems(rootFolders, d.Id, userId);
                while (rootFolders.NextPageRequest != null)
                {
                    rootFolders = await rootFolders.NextPageRequest.GetAsync();
                   await  PrintDriveItems(rootFolders, d.Id, userId);
                }
            }
        }
        private async Task PrintDriveItems(IDriveItemChildrenCollectionPage folders, string driveId, string userId)
        {
            foreach (var f in folders)
            {
                if (f.Folder == null) continue;
                Console.WriteLine(string.Format("Folder Name = {0}, Folder Id = {1}", f.Name, f.Id));
                var rootFolders = await graphServiceClient.Users[userId].Drives[driveId].Items[f.Id].Children
                                                .Request()
                                                .GetAsync();                
                Console.WriteLine("** Available files **");
                PrintFiles(rootFolders, driveId, userId);
                while (rootFolders.NextPageRequest != null)
                {
                    rootFolders = await rootFolders.NextPageRequest.GetAsync();
                    PrintFiles(rootFolders, driveId, userId);
                }
                Console.WriteLine("\r\n");
            }
        }
        private void PrintFiles(IDriveItemChildrenCollectionPage folders, string driveId, string userId)
        {
            foreach (var f in folders)
            {
                if (f.File == null) continue;
                Console.WriteLine(string.Format("File Name = {0}, File Id = {1}, Size = {2}, CreatedDateTime = {3}", f.Name, f.Id, f.Size, f.CreatedDateTime));                
            }
        }
        private GraphServiceClient GetGraphClient()
        {            
            var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                // get an access token for Graph
                var accessToken = GetAccessToken();

                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken.Result);

                return Task.FromResult(0);
            }));            
            return graphClient;
        }

        private async Task<string> GetAccessToken()
        {
            var url = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", _appConfig.TenantId);
            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "client_id", _appConfig.    AppId },
              { "grant_type", "client_credentials" },
              { "client_secret", _appConfig.AppSecret},
              { "scope", "https://graph.microsoft.com/.default" }
            });
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url))
            {
                Content = content
            };

            using (var response = await client.SendAsync(httpRequestMessage))
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                Office365TokenResponse myDeserializedClass = System.Text.Json.JsonSerializer.Deserialize<Office365TokenResponse>(responseStream);
                var token = myDeserializedClass.access_token;
                return token;
            }

        }
    }
}
