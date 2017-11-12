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
        public async Task Say()
        {
            string resultStr = null;

            resultStr += "Upcoming raids from " + _config["ladanse:api:siteName"] + "\n\n";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + _config["ladanse:api:secret"]);

                var response = httpClient.GetStringAsync(new Uri(_config["ladanse:api:baseUrl"])).Result;
                
                var eventsPage = JObject.Parse(response);

                var events = eventsPage["events"];

                if (!events.Any())
                {
                    resultStr += "No upcoming raids\n\n";

                    await ReplyAsync(resultStr);
                }

                foreach(var eventObj in events.Children())
                {
                    var eventName = eventObj["name"];
                    var eventInviteTime = eventObj["inviteTime"];

                    resultStr += $"**{eventName}** on {eventInviteTime}\n";
                }
            }

            await ReplyAsync(resultStr);
        }
    }
            
}