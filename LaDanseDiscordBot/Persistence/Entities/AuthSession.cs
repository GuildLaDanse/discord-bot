using System;
using System.Collections.Generic;
using System.Text;

namespace LaDanseDiscordBot.Persistence.Entities
{
    public enum AuthSessionState
    {
        Pending,
        Consumed,
        Removed
    }

    public class AuthSession
    {
        public int AuthSessionId { get; set; }

        public string Nonce { get; set; }

        public long CreatedOn { get; set; }

        public AuthSessionState State { get; set; }

        public DiscordUser DiscordUser { get; set; }
    }
}
