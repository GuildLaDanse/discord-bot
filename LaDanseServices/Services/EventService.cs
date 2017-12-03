using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaDanseRestTransport;
using LaDanseServices.Dto.Event;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;

namespace LaDanseServices.Services
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

        public async Task<EventPage> GetEvents()
        {
            try
            {
                var discordSiteResponse = await _laDanseRestClient.GetAsync<EventPage>(_laDanseUrlBuilder.QueryEventsUrl(), null);
                
                var eventsPage = discordSiteResponse.Body;

                return eventsPage;
            }
            catch (Exception e)
            {
               _logger.LogError("Failed to get events", e.StackTrace);
                _logger.LogError(e.ToString());
            }
            
            return null;
        }
    }
}