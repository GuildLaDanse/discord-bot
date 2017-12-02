using Microsoft.Extensions.Configuration;

namespace LaDanseRestTransport
{
    public class LaDanseUrlBuilder
    {
        private readonly IConfiguration _config;

        public LaDanseUrlBuilder(IConfiguration config)
        {
            _config = config;
        }

        public string GetStartUrl()
        {
            return _config["ladanse:api:baseUrl"];
        }

        public string CreateGetEventDetail(int eventId)
        {
            return ConstructFqUrl("/app/events#/events/event/" + eventId);
        }

        private string ConstructFqUrl(string contextPath)
        {
            return _config["ladanse:api:baseUrl"] + contextPath;
        }

        public string QueryEventsUrl()
        {
            return ConstructFqUrl("/api/events/");
        }

        public string QueryEventByIdUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}");
        }

        public string PostEventUrl()
        {
            return ConstructFqUrl("/api/events/");
        }

        public string PutEventUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}");
        }

        public string PutEventStateUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}/state");
        }

        public string DeleteEventUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}");
        }

        public string PostSignUpUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}/signUps");
        }

        public string PutSignUpUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}/signUps/{signUpId}");
        }

        public string DeleteSignUpUrl()
        {
            return ConstructFqUrl("/api/events/{eventId}/signUps/{signUpId}");
        }

        public string GetAllGameClassesUrl()
        {
            return ConstructFqUrl("/api/gamedata/gameClasses/");
        }

        public string GetAllGameFactionsUrl()
        {
            return ConstructFqUrl("/api/gamedata/gameFactions/");
        }

        public string GetAllGameRacesUrl()
        {
            return ConstructFqUrl("/api/gamedata/gameRaces/");
        }

        public string GetAllRealmsUrl()
        {
            return ConstructFqUrl("/api/gamedata/realms/");
        }

        public string PostRealmUrl()
        {
            return ConstructFqUrl("/api/gamedata/realms/");
        }

        public string GetAllGuildsUrl()
        {
            return ConstructFqUrl("/api/guilds/");
        }

        public string PostGuildUrl()
        {
            return ConstructFqUrl("/api/guilds/");
        }

        public string GetCharacterUrl()
        {
            return ConstructFqUrl("/api/characters/{characterId}");
        }

        public string PostClaimUrl()
        {
            return ConstructFqUrl("/api/characters/{characterId}/claim");
        }

        public string PutClaimUrl()
        {
            return ConstructFqUrl("/api/characters/{characterId}/claim");
        }

        public string DeleteClaimUrl()
        {
            return ConstructFqUrl("/api/characters/{characterId}/claim");
        }

        public string GetCharactersByCriteriaUrl()
        {
            return ConstructFqUrl("/api/queries/charactersByCriteria");
        }

        public string GetCharactersClaimedByAccountUrl()
        {
            return ConstructFqUrl("/api/queries/charactersClaimedByAccount");
        }

        public string GetCharactersInGuildUrl()
        {
            return ConstructFqUrl("/api/queries/charactersInGuild");
        }

        public string ProfileUrl()
        {
            return ConstructFqUrl("/api/profile/");
        }

        public string DiscordGrantUrl(string authCode)
        {
            return ConstructFqUrl("/api/discord/grant?authCode=" + authCode);
        }

        public string GetDiscordAuthInform(string nonce, string redirectUrl)
        {
            return ConstructFqUrl("/authorization/discord/inform?nonce=" + nonce + "&redirect=" + redirectUrl);
        }
    }
}