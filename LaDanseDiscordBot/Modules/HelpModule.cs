using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LaDanseRestTransport;
using Microsoft.Extensions.Configuration;

namespace LaDanseDiscordBot.Modules
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;

        public HelpModule(LaDanseUrlBuilder laDanseUrlBuilder)
        {
            _laDanseUrlBuilder = laDanseUrlBuilder;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            await ShowWebsitePage();
        }

        [Command("help")]
        public async Task HelpAsync(string command)
        {
            await ShowWebsitePage();
        }

        private async Task ShowWebsitePage()
        {
            var helpUrl = _laDanseUrlBuilder.GetDiscordHelpUrl();
            
            string resultStr = null;

            resultStr += $"Visit {helpUrl} to learn how I can help you.";

            await ReplyAsync(resultStr);
        }
    }
}