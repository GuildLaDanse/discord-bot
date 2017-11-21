using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LaDanseRestAPI.Dto.Reference
{
    public class CommentGroupReference
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
