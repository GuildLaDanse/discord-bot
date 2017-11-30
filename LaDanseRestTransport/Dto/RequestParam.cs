using Newtonsoft.Json;

namespace LaDanseRestTransport.Dto
{
    public class RequestParam
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
