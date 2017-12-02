using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BotCommon;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Persistence.Entities;
using LaDanseRestTransport;
using LaDanseServices.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace LaDanseDiscordBot.Modules
{
    [Name("Hello")]
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        private readonly LaDanseUrlBuilder _ldmUrlBuilder;
        private readonly DbContextFactory _dbContextFactory;

        public HelloModule(LaDanseUrlBuilder ldmUrlBuilder, DbContextFactory dbContextFactory)
        {
            _ldmUrlBuilder = ldmUrlBuilder;
            _dbContextFactory = dbContextFactory;
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
                    Console.WriteLine("User does not yet exist, creating it");
                    
                    discordUser = new DiscordUser {DiscordUserId = Context.User.Id};   
                    context.DiscordUsers.Add(discordUser);
                }

                #endregion

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

                context.SaveChanges();
                
                await Context.User.SendMessageAsync(
                    _ldmUrlBuilder.GetDiscordAuthInform(newAuthSession.Nonce, "http://localhost:57077/connect/website"));
            }
        }
    }
            
}