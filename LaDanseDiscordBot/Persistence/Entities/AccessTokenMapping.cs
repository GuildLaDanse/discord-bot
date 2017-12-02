using System;
using System.Collections.Generic;
using System.Text;

namespace LaDanseDiscordBot.Persistence.Entities
{
    public enum AccessTokenState
    {
        Active,
        Removed
    }

    public class AccessTokenMapping
    {
        public int AccessTokenMappingId { get; set; }

        public string AccessToken { get; set; }

        public long CreatedOn { get; set; }

        public AccessTokenState State { get; set; }

        public DiscordUser DiscordUser { get; set; }
    }
}
