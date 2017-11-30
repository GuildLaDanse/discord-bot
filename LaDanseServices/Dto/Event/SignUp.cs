using System.Collections.Generic;
using LaDanseServices.Dto.Reference;
using Newtonsoft.Json;

namespace LaDanseServices.Dto.Event
{
    public class SignUp
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("accountRef")]
        public AccountReference Account { get; set; }
    }
}
