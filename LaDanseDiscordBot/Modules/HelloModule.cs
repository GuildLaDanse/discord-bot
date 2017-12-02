using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
                var discordUser = new DiscordUser {DiscordUserId = Context.User.Id};

                var authSession = new AuthSession
                {
                    Nonce = "ThisIsANonce",
                    CreatedOn = 0,
                    DiscordUser = discordUser,
                    State = AuthSessionState.Pending
                };

                context.DiscordUsers.Add(discordUser);
                context.AuthSessions.Add(authSession);

                context.SaveChanges();

                await Context.User.SendMessageAsync(
                    _ldmUrlBuilder.GetDiscordAuthInform(authSession.Nonce, "http://localhost:57077/"));
            }
        }
    }
            
}