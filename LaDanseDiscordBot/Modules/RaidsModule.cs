using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace LaDanseDiscordBot.Modules
{
    [Name("Raids")]
    public class RaidsModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public RaidsModule(IConfigurationRoot config)
        {
            _config = config;
        }

        [Command("raids"), Alias("events")]
        [Summary("List all future raids and events for the next month")]
        public async Task Raids()
        {
            string resultStr = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + _config["ladanse:api:secret"]);

                var response = httpClient.GetStringAsync(new Uri(_config["ladanse:api:baseUrl"] + "/discord/api/request")).Result;
                
                var eventsPage = JObject.Parse(response);
                
                var events = eventsPage["events"];

                if (!events.Any())
                {
                    resultStr += "No upcoming raids scheduled on " + _config["ladanse:api:baseUrl"] + "\n\n";

                    await ReplyAsync(resultStr);

                    return;
                }

                resultStr += "Upcoming raids from " + _config["ladanse:api:baseUrl"] + "\n\n";

                foreach (var eventObj in events.Children())
                {
                    var eventName = eventObj["name"];
                    var jtEventInviteTime = eventObj["inviteTime"];
                   
                    var eventInviteTime = Convert.ToDateTime(jtEventInviteTime.ToString());

                    var eventUrl = _config["ladanse:api:baseUrl"] + "/app/events#/events/event/" + eventObj["id"];

                    resultStr += $"**{eventName}** on {eventInviteTime:ddd d/M}\n{eventUrl}\n\n";
                }
            }

            await ReplyAsync(resultStr);
        }

        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign()
        {
            string resultStr = null;

            resultStr += $"I would love to sign you up, {Context.User.Username}, but you need to give me more information ...\n\n";
            resultStr += "Try something like !sign me up for 17/02 as healer and dps";

            await ReplyAsync(resultStr);
        }

        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign([Remainder]string text)
        {
            await ReplyAsync($"Sorry {Context.User.Username}, this functionality is not yet implemented. Stay tuned!");
        }
    }
            
}