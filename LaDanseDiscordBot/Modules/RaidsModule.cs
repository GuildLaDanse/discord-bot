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
    [Name("Raids")]
    public class RaidsModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly EventService _eventService;
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;
        private readonly ILogger _logger;

        public RaidsModule(
            IConfiguration config, 
            EventService eventService, 
            LaDanseUrlBuilder laDanseUrlBuilder,
            ILogger<RaidsModule> logger)
        {
            _config = config;
            _eventService = eventService;
            _laDanseUrlBuilder = laDanseUrlBuilder;
            _logger = logger;
        }

        [Command("raids"), Alias("events")]
        [Summary("List all future raids for the next month")]
        public async Task Raids()
        {
            string resultStr = null;

            var eventsPage = await _eventService.GetEvents();

            if (!eventsPage.Events.Any())
            {
                resultStr += "No upcoming raids scheduled on " + _config["ladanse:api:baseUrl"] + "\n\n";

                await ReplyAsync(resultStr);

                return;
            }
            
            resultStr += "Upcoming raids from " + _config["ladanse:api:baseUrl"] + "\n\n";
            
            foreach (var eventObj in eventsPage.Events)
            {
                var eventName = eventObj.Name;

                var eventInviteTime = TimeUtils.ToRealmTimeZone(eventObj.InviteTime);

                var eventUrl = _laDanseUrlBuilder.CreateGetEventDetail(eventObj.Id);

                var willComeCount = 0;
                var mightComeCount = 0;
                var absenceCount = 0;

                foreach (var signUp in eventObj.SignUps)
                {
                    switch (signUp.Type)
                    {
                        case "WillCome":
                            willComeCount++;
                            break;
                        case "MightCome":
                            mightComeCount++;
                            break;
                        case "Absent":
                            absenceCount++;
                            break;
                        default:
                            break;
                    }
                }

                resultStr += $"**{eventName}** on " +
                             $"{eventInviteTime:ddd d/M HH:mm} - " +
                             $"({willComeCount}/{mightComeCount} - {absenceCount})" +
                             $"\n{eventUrl}\n\n";
            }

            await ReplyAsync(resultStr);
        }
    }
}