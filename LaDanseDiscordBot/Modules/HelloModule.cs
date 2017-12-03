using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Persistence.Entities;
using LaDanseRestTransport;
using LaDanseServices.Dto.Profile;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot.Modules
{
    [Name("Hello")]
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        private readonly LaDanseRestClient _laDanseRestClient;
        private readonly LaDanseUrlBuilder _ladanseUrlBuilder;
        private readonly DbContextFactory _dbContextFactory;
        private readonly ILogger _logger;

        public HelloModule(
            LaDanseRestClient laDanseRestClient,
            LaDanseUrlBuilder ladanseUrlBuilder, 
            DbContextFactory dbContextFactory,
            ILogger<HelloModule> logger)
        {
            _laDanseRestClient = laDanseRestClient;
            _ladanseUrlBuilder = ladanseUrlBuilder;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        [Command("hello")]
        [Summary("Become friends with the bot")]
        public async Task Hello()
        {
            using (var dbContext = _dbContextFactory.CreateContext())
            {
                var discordUserId = Context.User.Id;

                #region Fetch Discord User

                var discordUser = dbContext.DiscordUsers.Find(discordUserId);

                if (discordUser == null)
                {
                    _logger.LogInformation("User does not yet exist, creating it");
                    
                    discordUser = new DiscordUser {DiscordUserId = Context.User.Id};   
                    dbContext.DiscordUsers.Add(discordUser);
                }

                var accessToken = GetAccessToken(dbContext, discordUser);
                
                if (accessToken != null)
                {
                    if (await IsValidAccessToken(accessToken))
                    {
                        await HelloKnownUser(dbContext, discordUser);    
                    }
                    else
                    {
                        await HelloForgottenUser(dbContext, discordUser);
                    }
                }
                else
                {
                    await HelloUnknownUser(dbContext, discordUser);
                }

                #endregion

                dbContext.SaveChanges();
            }
        }

        private string GetAccessToken(DiscordBotContext dbContext, DiscordUser discordUser)
        {
            var accessTokenMapping = dbContext.AccessTokenMappings
                .FirstOrDefault(a => a.DiscordUser == discordUser && a.State == AccessTokenState.Active);

            return accessTokenMapping?.AccessToken;
        }

        private async Task<bool> IsValidAccessToken(string accessToken)
        {
            var profileResponse = await _laDanseRestClient.GetAsync<Profile>(_ladanseUrlBuilder.ProfileUrl(), accessToken);

            return (profileResponse.IsSuccess && profileResponse.Body != null &&
                    profileResponse.Body.GetType() == typeof(Profile));
        }
        
        private async Task HelloKnownUser(DiscordBotContext dbContext, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, nice to see you. How are you doing?");
        }
        
        private async Task HelloForgottenUser(DiscordBotContext dbContext, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, I remember you but I seem to have forgotten some of our adventure. " +
                             $"I have reached out to you privately to get to know each other again.");
            
            await GetToKnowHelper.GetToKnowUser(Context, _ladanseUrlBuilder, dbContext, discordUser);
        }

        private async Task HelloUnknownUser(DiscordBotContext dbContext, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, it seems we don't know each other yet. " +
                             $"I have reached out to you privately to get to known each better.");

            await GetToKnowHelper.GetToKnowUser(Context, _ladanseUrlBuilder, dbContext, discordUser);
        }
    }
            
}