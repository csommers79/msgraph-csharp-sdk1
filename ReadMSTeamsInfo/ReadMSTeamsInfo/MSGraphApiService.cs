using DomainObjects.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Channels;

namespace DomainObjects.Services
{
    public class MSGraphApiService
    {
        private static readonly object _lock = new object();
        private static MSGraphApiService? _instance = null;
        private AppConfig _appConfig;
        static readonly HttpClient client = new HttpClient();
        private GraphServiceClient? graphServiceClient = null;
        private MSGraphApiService(AppConfig appConfig)
        {
            _appConfig = appConfig;            
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

        public async Task ReadTeamInfo()
        {
            try
            {
                this.graphServiceClient = GetGraphClient();
                {
                                       
                    try
                    {
                        var allTeams = await graphServiceClient.Teams.Request().GetAsync();

                        Console.WriteLine("** Available teams **");
                        await PrintTeams(allTeams);
                        while (allTeams.NextPageRequest != null)
                        {
                            allTeams = await allTeams.NextPageRequest.GetAsync();
                           await PrintTeams(allTeams);
                        }   
                        

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            catch (ServiceException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


       private async Task PrintTeams(IGraphServiceTeamsCollectionPage teamList)
        { 
            foreach(var team in teamList)
            {
                Console.WriteLine(string.Format("Team display name = {0}", team.DisplayName));
                var channels = await graphServiceClient?.Teams[team.Id].Channels.Request().GetAsync();
                await PrintTeamChannels(channels, team.Id);
                while (channels.NextPageRequest != null)
                {
                    channels = await channels.NextPageRequest.GetAsync();
                   await PrintTeamChannels(channels, team.Id);
                }
            }            
        }
        private async Task PrintTeamChannels(ITeamChannelsCollectionPage channelList, string teamId)
        {
            foreach (var channel in channelList)
            {
                Console.WriteLine(string.Format("   Channel display name = {0}", channel.DisplayName));
                var members = await graphServiceClient?.Teams[teamId]?.Channels[channel.Id]?.Members?.Request().GetAsync();
                PrintChannelMembers(members);
                while (members.NextPageRequest != null)
                {
                    members = await members.NextPageRequest.GetAsync();
                    PrintChannelMembers(members);
                }
                if (channel.DisplayName == _appConfig.ChannelName) // read the messages for specified channel
                {

                    var messages = await graphServiceClient?.Teams[teamId]?.Channels[channel.Id]?.Messages?.Request().GetAsync();
                    await PrintChannelMessages(messages, teamId, channel.Id);
                    while (messages.NextPageRequest != null)
                    {
                        messages = await messages.NextPageRequest.GetAsync();
                        await PrintChannelMessages(messages, teamId, channel.Id);
                    }

                }
            }            
        }

        private void PrintChannelMembers(IChannelMembersCollectionPage members)
        {
            foreach (var member in members)
            {
                Console.WriteLine(string.Format("       Member display name = {0}", member.DisplayName));               
            }
        }
        private async Task PrintChannelMessages(IChannelMessagesCollectionPage messages, string teamId, string channelId)
        {
            foreach (var msg in messages)
            {
                Console.WriteLine(string.Format("       Message = {0}, From= {1}", msg.Body?.Content, msg.From.User.DisplayName));
                var replies = await graphServiceClient?.Teams[teamId].Channels[channelId].Messages[msg.Id].Replies.Request().GetAsync();
                await PrintMessageReplies(replies, teamId, channelId);
                while (replies.NextPageRequest != null)
                {
                    replies = await replies.NextPageRequest.GetAsync();
                    await PrintMessageReplies(replies, teamId, channelId);
                }
            }
        }
        private async Task PrintMessageReplies(IChatMessageRepliesCollectionPage messages, string teamId, string channelId)
        {
            foreach (var msg in messages)
            {
                Console.WriteLine(string.Format("       Message = {0}, From= {1}", msg.Body?.Content, msg.From.User.DisplayName));
                var replies = await graphServiceClient?.Teams[teamId].Channels[channelId].Messages[msg.Id].Replies.Request().GetAsync();
                await PrintMessageReplies(replies, teamId, channelId);
                while (replies.NextPageRequest != null)
                {
                    replies = await replies.NextPageRequest.GetAsync();
                    await PrintMessageReplies(replies, teamId, channelId);
                }
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
