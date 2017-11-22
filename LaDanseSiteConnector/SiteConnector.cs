using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LaDanseSiteConnector.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LaDanseSiteConnector
{
    public class SiteConnector
    {
        private readonly IConfigurationRoot _config;

        public SiteConnector(IConfigurationRoot config)
        {
            _config = config;
        }

        public async Task<TReturn> MakeRequest<TReturn>(
            string actionName,
            Dictionary<string, string> parameters,
            int? impersonateUser)
        {
            var discordSiteRequest = new DiscordSiteRequest
            {
                Action = actionName,
                ImpersonateUser = impersonateUser
            };

            return await MakeRequest<TReturn>(discordSiteRequest);
        }

        public async Task<TReturn> MakeRequest<TReturn, TBody>(
            string actionName, 
            TBody body, 
            Dictionary<string, string> parameters, 
            int? impersonateUser)
        {
            var discordSiteRequest = new DiscordSiteRequest<TBody>
            {
                Action = actionName,
                ImpersonateUser = impersonateUser,
                Body = body
            };

            return await MakeRequest<TReturn>(discordSiteRequest);
        }

        public async Task MakeRequest<TBody>(
            string actionName,
            TBody body,
            Dictionary<string, string> parameters,
            int? impersonateUser)
        {
            var discordSiteRequest = new DiscordSiteRequest<TBody>
            {
                Action = actionName,
                ImpersonateUser = impersonateUser,
                Body = body
            };

            await MakeRequest(discordSiteRequest);
        }

        private async Task<TReturn> MakeRequest<TReturn>(
            DiscordSiteRequest discordSiteRequest)
        {
            System.Console.WriteLine(JsonConvert.SerializeObject(discordSiteRequest, Formatting.Indented));

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + _config["ladanse:api:secret"]);

                var response = await httpClient.GetStringAsync(new Uri(_config["ladanse:api:baseUrl"] + "/discord/api/request"));

                return JsonConvert.DeserializeObject<TReturn>(response);
            }
        }

        private async Task MakeRequest(
            DiscordSiteRequest discordSiteRequest)
        {
            System.Console.WriteLine(JsonConvert.SerializeObject(discordSiteRequest, Formatting.Indented));

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + _config["ladanse:api:secret"]);

                await httpClient.GetStringAsync(new Uri(_config["ladanse:api:baseUrl"] + "/discord/api/request"));
            }
        }
    }
}
