using System;
using Newtonsoft.Json;

namespace LaDanseServices.Dto.Reference
{
    public class AccountReference
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }
    }
}
