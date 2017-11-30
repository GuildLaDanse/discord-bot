using System;
using System.Collections.Generic;
using LaDanseServices.Dto.Reference;
using Newtonsoft.Json;

namespace LaDanseServices.Dto.Event
{
    public class Event
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("organiserRef")]
        public AccountReference Organiser { get; set; }

        [JsonProperty("inviteTime")]
        public DateTime InviteTime { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("signUps")]
        public List<SignUp> SignUps { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("commentGroupRef")]
        public CommentGroupReference CommentGroup { get; set; }
    }
}
