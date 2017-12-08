using System.ComponentModel;
using LaDanseDiscordBot.Persistence.Entities;

namespace LaDanseDiscordBot.Modules
{
    public class LaDanseCommandContext
    {
        public enum DiscordUserState
        {
            New, Unknown, Known, Forgotten
        }
        
        public DiscordUser DiscordUser { get;  }
        public string AccessToken { get; }
        public DiscordUserState UserState { get; }
        public int? SiteAccountId { get; }

        public LaDanseCommandContext(DiscordUser discordUser, string accessToken, DiscordUserState userState, int? siteAccountId)
        {
            DiscordUser = discordUser;
            AccessToken = accessToken;
            UserState = userState;
            SiteAccountId = siteAccountId;
        }

        public bool IsUserKnownAndMapped()
        {
            return AccessToken != null;
        }
    }
}