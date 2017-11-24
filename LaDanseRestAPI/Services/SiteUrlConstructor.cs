using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LaDanseRestAPI.Services
{
    public class SiteUrlConstructor
    {
        private readonly IConfigurationRoot _config;

        public SiteUrlConstructor(IConfigurationRoot config)
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
    }
}