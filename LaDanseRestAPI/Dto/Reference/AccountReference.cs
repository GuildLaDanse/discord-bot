using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LaDanseRestAPI.Dto.Reference
{
    public class AccountReference
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }
    }
}
