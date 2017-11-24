using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaDanseRestAPI.Dto.Event;
using LaDanseSiteConnector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaDanseRestAPI.Services
{
    public class RaidService
    {
        private readonly SiteUrlConstructor _siteUrlConstructor;
        private readonly SiteConnector _siteConnector;
        private readonly ILogger _logger;

        public RaidService(SiteUrlConstructor siteUrlConstructor, SiteConnector siteConnector, ILogger<RaidService> logger)
        {
            _siteUrlConstructor = siteUrlConstructor;
            _siteConnector = siteConnector;
            _logger = logger;
        }

        public async Task<List<string>> GetRaids()
        {
            var eventsPage = await _siteConnector.MakeRequest<EventPage>("GetEvents", null, null);

            var result = new List<string>();

            foreach (var eventObj in eventsPage.Events)
            {
                var eventName = eventObj.Name;

                var eventInviteTime = ToRealmTimeZone(eventObj.InviteTime);

                var eventUrl = _siteUrlConstructor.CreateGetEventDetail(eventObj.Id);

                _logger.LogInformation("Found event, added to response");

                result.Add($"**{eventName}** on {eventInviteTime:ddd d/M - HH:mm}\n{eventUrl}\n\n");
            }

            return result;
        }

        private DateTime ToRealmTimeZone(DateTime origDateTime)
        {
            return TimeZoneInfo.ConvertTime(origDateTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
        }
    }
}