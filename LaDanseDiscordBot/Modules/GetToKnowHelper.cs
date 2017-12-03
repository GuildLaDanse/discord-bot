using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using Discord;
using Discord.Commands;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Persistence.Entities;
using LaDanseRestTransport;

namespace LaDanseDiscordBot.Modules
{
    public static class GetToKnowHelper
    {
        public static async Task GetToKnowUser(
            SocketCommandContext socketCommandContext,
            LaDanseUrlBuilder laDanseUrlBuilder,
            DiscordBotContext dbContext, 
            DiscordUser discordUser)
        {
            #region CleanUpAuthSessions

            var authSessions = dbContext.AuthSessions
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

            dbContext.AuthSessions.Add(newAuthSession);

            #endregion
            
            await socketCommandContext.User.SendMessageAsync(
                "Click on this URL and follow the instructions if you want us to get to know each other better ...\n\n" + 
                laDanseUrlBuilder.GetDiscordAuthInformUrl(newAuthSession.Nonce, "http://localhost:57077/connect/website"));
        }
    }
}