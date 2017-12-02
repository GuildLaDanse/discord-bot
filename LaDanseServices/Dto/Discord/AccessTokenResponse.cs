using Newtonsoft.Json;

namespace LaDanseServices.Dto.Discord
{
    public class AccessTokenResponse
    {
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}