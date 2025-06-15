using Azure;
using Azure.Communication;
using Azure.Communication.Identity;

namespace Ora.GameManaging.Mafia.Infrastructure.Services
{
    public class AzureService(IConfiguration configuration)
    {
        private readonly string connectionString = configuration.GetConnectionString("AzureCommunicationService");

        public async Task<(string, string, DateTimeOffset)> CreateUserAndToken()
        {
            var connectionString = configuration.GetConnectionString("AzureCommunicationService");
            var client = new CommunicationIdentityClient(connectionString);

            Response<CommunicationUserIdentifier> communicationUser = await client.CreateUserAsync();
            //RoomParticipant participant = new(communicationUser.Value);
            var token = await client.GetTokenAsync(new CommunicationUserIdentifier(communicationUser.Value.Id), scopes: [CommunicationTokenScope.VoIP], new TimeSpan(24, 0, 0));
            var tokenValue = token.Value.Token;
            return (communicationUser.Value.Id, tokenValue, token.Value.ExpiresOn);
        }
        public async Task<string> CreateUserAsync()
        {
            var client = new CommunicationIdentityClient(connectionString);

            Response<CommunicationUserIdentifier> communicationUser = await client.CreateUserAsync();
            return communicationUser.Value.Id;
        }

        public async Task<(string Token, DateTimeOffset ExpiresOn)> GetTokenAsync(string acsUserId)
        {
            var client = new CommunicationIdentityClient(connectionString);

            var user = new CommunicationUserIdentifier(acsUserId);
            var tokenResponse = await client.GetTokenAsync(user, scopes: [CommunicationTokenScope.VoIP]);
            return (tokenResponse.Value.Token, tokenResponse.Value.ExpiresOn);
        }
    }
}
