using System.Collections.Generic;
using Newtonsoft.Json;

namespace LaDanseRestAPI.Dto.Event
{
    public class EventPage
    {
        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        [JsonProperty("previousTimestamp")]
        public string PreviousTimestamp { get; set; }

        [JsonProperty("nextTimestamp")]
        public string NextTimestamp { get; set; }
    }
}
