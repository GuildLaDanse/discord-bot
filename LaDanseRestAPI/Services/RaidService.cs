using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaDanseRestAPI.Dto.Event;
using LaDanseSiteConnector;
using Microsoft.Extensions.Configuration;

namespace LaDanseRestAPI.Services
{
    public class RaidService
    {
        private readonly IConfigurationRoot _config;
        private readonly SiteConnector _siteConnector;

        public RaidService(IConfigurationRoot config, SiteConnector siteConnector)
        {
            _config = config;
            _siteConnector = siteConnector;
        }

        public async Task<List<string>> GetRaids()
        {
            var eventsPage = await _siteConnector.MakeRequest<EventPage>("GetEvents", null, null);

            var result = new List<string>();

            foreach (var eventObj in eventsPage.Events)
            {
                var eventName = eventObj.Name;

                var eventInviteTime = eventObj.InviteTime;

                var eventUrl = _config["ladanse:api:baseUrl"] + "/app/events#/events/event/" + eventObj.Id;

                result.Add($"**{eventName}** on {eventInviteTime:ddd d/M}\n{eventUrl}\n\n");
            }

            return result;
        }
    }
}