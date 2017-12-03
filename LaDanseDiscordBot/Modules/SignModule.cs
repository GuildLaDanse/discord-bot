using System.Linq;
using System.Threading.Tasks;
using BotCommon;
using Discord.Commands;
using LaDanseRestTransport;
using LaDanseServices.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot.Modules
{
    [Name("Sign")]
    public class SignModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly EventService _eventService;
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;
        private readonly ILogger _logger;

        public SignModule(
            IConfiguration config, 
            EventService eventService, 
            LaDanseUrlBuilder laDanseUrlBuilder,
            ILogger<SignModule> logger)
        {
            _config = config;
            _eventService = eventService;
            _laDanseUrlBuilder = laDanseUrlBuilder;
            _logger = logger;
        }
        
        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign([Remainder] string text)
        {
            await ReplyAsync($"Sorry {Context.User.Username}, this functionality is not yet implemented. Stay tuned!");
        }

        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign()
        {
            var helpUrl = _laDanseUrlBuilder.GetDiscordHelpUrl();
            
            string resultStr = null;

            resultStr += $"I would love to sign you up, {Context.User.Username}, " +
                         $"but you need to give me more information ...\n\n" +
                         $"Try something like !sign me up for 17/02 as healer and dps";

            await ReplyAsync(resultStr);
        }
    }
}