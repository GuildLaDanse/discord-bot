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
            return "/api/events/";
        }

        public string QueryEventByIdUrl()
        {
            return "/api/events/{eventId}";
        }

        public string PostEventUrl()
        {
            return "/api/events/";
        }

        public string PutEventUrl()
        {
            return "/api/events/{eventId}";
        }

        public string PutEventStateUrl()
        {
            return "/api/events/{eventId}/state";
        }

        public string DeleteEventUrl()
        {
            return "/api/events/{eventId}";
        }

        public string PostSignUpUrl()
        {
            return "/api/events/{eventId}/signUps";
        }

        public string PutSignUpUrl()
        {
            return "/api/events/{eventId}/signUps/{signUpId}";
        }

        public string DeleteSignUpUrl()
        {
            return "/api/events/{eventId}/signUps/{signUpId}";
        }

        public string GetAllGameClassesUrl()
        {
            return "/api/gamedata/gameClasses/";
        }

        public string GetAllGameFactionsUrl()
        {
            return "/api/gamedata/gameFactions/";
        }

        public string GetAllGameRacesUrl()
        {
            return "/api/gamedata/gameRaces/";
        }

        public string GetAllRealmsUrl()
        {
            return "/api/gamedata/realms/";
        }

        public string PostRealmUrl()
        {
            return "/api/gamedata/realms/";
        }

        public string GetAllGuildsUrl()
        {
            return "/api/guilds/";
        }

        public string PostGuildUrl()
        {
            return "/api/guilds/";
        }

        public string GetCharacterUrl()
        {
            return "/api/characters/{characterId}";
        }

        public string PostClaimUrl()
        {
            return "/api/characters/{characterId}/claim";
        }

        public string PutClaimUrl()
        {
            return "/api/characters/{characterId}/claim";
        }

        public string DeleteClaimUrl()
        {
            return "/api/characters/{characterId}/claim";
        }

        public string GetCharactersByCriteriaUrl()
        {
            return "/api/queries/charactersByCriteria";
        }

        public string GetCharactersClaimedByAccountUrl()
        {
            return "/api/queries/charactersClaimedByAccount";
        }

        public string GetCharactersInGuildUrl()
        {
            return "/api/queries/charactersInGuild";
        }

        public string GetDiscordAuthInform(string nonce, string redirectUrl)
        {
            return ConstructFqUrl("/authorization/discord/inform?nonce=" + nonce + "&redirect=" + redirectUrl);
        }
    }
}