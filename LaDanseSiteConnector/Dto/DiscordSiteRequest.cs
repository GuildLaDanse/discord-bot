using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LaDanseSiteConnector.Dto
{
    public class DiscordSiteRequest
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("impersonateUser")]
        public int? ImpersonateUser { get; set; }
        
        [JsonProperty("params")]
        public List<RequestParam> RequestParams { get; set; }
    }

    public class DiscordSiteRequest<TBody> : DiscordSiteRequest
    {
        [JsonProperty("body")]
        public Object Body { get; set; }
    }
}
