using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LaDanseRestAPI.Dto.Event;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LaDanseRestAPI.Services
{
    public class RaidService
    {
        private readonly IConfigurationRoot _config;

        public RaidService(IConfigurationRoot config)
        {
            _config = config;
        }

        public async Task<List<String>> GetRaids()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + _config["ladanse:api:secret"]);

                var response = await httpClient.GetStringAsync(new Uri(_config["ladanse:api:baseUrl"] + "/discord/api/request"));

                var eventsPage = JsonConvert.DeserializeObject<EventPage>(response);
                
                var result = new List<String>();

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
}