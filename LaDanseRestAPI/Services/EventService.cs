using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaDanseRestAPI.Dto.Event;
using LaDanseSiteConnector;
using Microsoft.Extensions.Logging;

namespace LaDanseRestAPI.Services
{
    public class EventService
    {
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;
        private readonly LaDanseRestClient _laDanseRestClient;
        private readonly ILogger _logger;

        public EventService(LaDanseUrlBuilder laDanseUrlBuilder, LaDanseRestClient laDanseRestClient, ILogger<EventService> logger)
        {
            _laDanseUrlBuilder = laDanseUrlBuilder;
            _laDanseRestClient = laDanseRestClient;
            _logger = logger;
        }

        public async Task<List<string>> GetRaids()
        {
            var result = new List<string>();

            try
            {
                var discordSiteResponse = await _laDanseRestClient.MakeRequest<EventPage>("queryEvents", null, null);
                
                var eventsPage = discordSiteResponse.Body;

                foreach (var eventObj in eventsPage.Events)
                {
                    var eventName = eventObj.Name;

                    var eventInviteTime = ToRealmTimeZone(eventObj.InviteTime);

                    var eventUrl = _laDanseUrlBuilder.CreateGetEventDetail(eventObj.Id);

                    _logger.LogInformation("Found event, added to response");

                    result.Add($"**{eventName}** on {eventInviteTime:ddd d/M - HH:mm}\n{eventUrl}\n\n");
                }
            }
            catch (Exception e)
            {
               _logger.LogError("Failed to get events", e);
            }
            
            return result;
        }

        private DateTime ToRealmTimeZone(DateTime origDateTime)
        {
            return TimeZoneInfo.ConvertTime(origDateTime, TimeZoneInfo.FindSystemTimeZoneById("CET"));
        }
    }
}