using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LaDanseDiscordBot.Modules;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Persistence.Entities;
using LaDanseRestTransport;
using LaDanseServices.Dto.Profile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot.Services
{
    public class LaDanseCommandContextFactory
    {
        private readonly IConfiguration _config;
        private readonly LaDanseRestClient _laDanseRestClient;
        private readonly LaDanseUrlBuilder _ladanseUrlBuilder;
        private readonly DbContextFactory _dbContextFactory;
        private readonly ILogger _logger;

        public LaDanseCommandContextFactory(
            IConfiguration config, 
            DbContextFactory dbContextFactory,
            LaDanseRestClient laDanseRestClient,
            LaDanseUrlBuilder ladanseUrlBuilder, 
            ILogger<SignModule> logger)
        {
            _config = config;
            _dbContextFactory = dbContextFactory;
            _laDanseRestClient = laDanseRestClient;
            _ladanseUrlBuilder = ladanseUrlBuilder;
            _logger = logger;
        }

        public async Task<LaDanseCommandContext> CreateContext(SocketCommandContext socketContext)
        {
            using (var dbContext = _dbContextFactory.CreateContext())
            {
                var discordUserId = socketContext.User.Id;

                #region Fetch Discord User

                var discordUser = dbContext.DiscordUsers.Find(discordUserId);

                if (discordUser == null)
                {
                    _logger.LogInformation("User does not yet exist, creating it");
                    
                    discordUser = new DiscordUser {DiscordUserId = socketContext.User.Id};   
                    dbContext.DiscordUsers.Add(discordUser);
                    
                    dbContext.SaveChanges();
                    
                    return new LaDanseCommandContext(discordUser, null, LaDanseCommandContext.DiscordUserState.New, null);
                }

                var accessToken = GetAccessToken(dbContext, discordUser);
                
                if (accessToken != null)
                {
                    var profile = await IsValidAccessToken(accessToken);
                    
                    if (profile != null)
                    {
                        return new LaDanseCommandContext(discordUser, accessToken, LaDanseCommandContext.DiscordUserState.Known, profile.Id);
                    }
                    else
                    {
                        return new LaDanseCommandContext(discordUser, accessToken, LaDanseCommandContext.DiscordUserState.Forgotten, null);
                    }
                }
                else
                {
                    return new LaDanseCommandContext(discordUser, null, LaDanseCommandContext.DiscordUserState.Unknown, null);
                }

                #endregion
            }
        }
        
        private static string GetAccessToken(DiscordBotContext dbContext, DiscordUser discordUser)
        {
            var accessTokenMapping = dbContext.AccessTokenMappings
                .FirstOrDefault(a => a.DiscordUser == discordUser && a.State == AccessTokenState.Active);

            return accessTokenMapping?.AccessToken;
        }
        
        private async Task<Profile> IsValidAccessToken(string accessToken)
        {
            var profileResponse = await _laDanseRestClient.GetAsync<Profile>(_ladanseUrlBuilder.ProfileUrl(), accessToken);

            return (profileResponse.IsSuccess && profileResponse.Body != null &&
                    profileResponse.Body.GetType() == typeof(Profile))? profileResponse.Body : null;
        }
    }
}