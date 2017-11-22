using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LaDanseSiteConnector.Dto
{
    public class RequestParam
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
