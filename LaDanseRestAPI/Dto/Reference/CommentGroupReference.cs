using Newtonsoft.Json;

namespace LaDanseRestAPI.Dto.Reference
{
    public class CommentGroupReference
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
