using Newtonsoft.Json;

namespace LaDanseServices.Dto.Profile
{
    public class Profile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}