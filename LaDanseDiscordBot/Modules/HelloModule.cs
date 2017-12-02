using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using Discord;
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
            using (var context = _dbContextFactory.CreateContext())
            {
                var discordUserId = Context.User.Id;

                #region Fetch Discord User

                var discordUser = context.DiscordUsers.Find(discordUserId);

                if (discordUser == null)
                {
                    _logger.LogInformation("User does not yet exist, creating it");
                    
                    discordUser = new DiscordUser {DiscordUserId = Context.User.Id};   
                    context.DiscordUsers.Add(discordUser);
                }

                var accessToken = GetAccessToken(context, discordUser);
                
                if (accessToken != null)
                {
                    if (await IsValidAccessToken(accessToken))
                    {
                        await HelloKnownUser(context, discordUser);    
                    }
                    else
                    {
                        await HelloForgottenUser(context, discordUser);
                    }
                }
                else
                {
                    await HelloUnknownUser(context, discordUser);
                }

                #endregion

                context.SaveChanges();
            }
        }

        private string GetAccessToken(DiscordBotContext context, DiscordUser discordUser)
        {
            var accessTokenMapping = context.AccessTokenMappings
                .FirstOrDefault(a => a.DiscordUser == discordUser && a.State == AccessTokenState.Active);

            return accessTokenMapping?.AccessToken;
        }

        private async Task<bool> IsValidAccessToken(string accessToken)
        {
            var profileResponse = await _laDanseRestClient.GetAsync<Profile>(_ladanseUrlBuilder.ProfileUrl(), accessToken);

            return (profileResponse.IsSuccess && profileResponse.Body != null &&
                    profileResponse.Body.GetType() == typeof(Profile));
        }
        
        private async Task HelloKnownUser(DiscordBotContext context, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, nice to see you. How are you doing?");
        }
        
        private async Task HelloForgottenUser(DiscordBotContext context, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, I remember you but I seem to have forgotten some of our adventure. " +
                             $"I have reached out to you privately to get to know each other again.");
            
            await GetToKnowUser(context, discordUser);
        }

        private async Task HelloUnknownUser(DiscordBotContext context, DiscordUser discordUser)
        {
            await ReplyAsync($"Hello {Context.User.Username}, it seems we don't know each other yet. " +
                             $"I have reached out to you privately to get to known each better.");

            await GetToKnowUser(context, discordUser);
        }

        private async Task GetToKnowUser(DiscordBotContext context, DiscordUser discordUser)
        {
            #region CleanUpAuthSessions

            var authSessions = context.AuthSessions
                .Where(b => b.DiscordUser == discordUser)
                .ToList();

            foreach (var authSession in authSessions)
            {
                if (authSession.State == AuthSessionState.Pending)
                    authSession.State = AuthSessionState.Removed;
            }
                
            #endregion

            #region NewAuthSession

            var newAuthSession = new AuthSession
            {
                Nonce = RandomStringUtils.Random(32),
                CreatedOn = 0,
                DiscordUser = discordUser,
                State = AuthSessionState.Pending
            };

            context.AuthSessions.Add(newAuthSession);

            #endregion
            
            await Context.User.SendMessageAsync(
                "Click on this URL and follow the instructions if you want us to get to know each other better ...\n\n" + 
                _ladanseUrlBuilder.GetDiscordAuthInform(newAuthSession.Nonce, "http://localhost:57077/connect/website"));
        }
    }
            
}