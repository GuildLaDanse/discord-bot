using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaDanseRestTransport.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LaDanseRestTransport
{
    public class LaDanseRestClient
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public LaDanseRestClient(IConfiguration config, ILogger<LaDanseRestClient> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<LaDanseRestResponse<TReturn>> GetAsync<TReturn>(string url)
        {
            return await MakeRequest<TReturn>();
        }
        
        public async Task<LaDanseRestResponse<TReturn>> MakeRequest<TReturn>(
            string actionName,
            Dictionary<string, string> parameters,
            int? impersonateUser)
        {
            return await MakeRequest<TReturn>();
        }

        public async Task<LaDanseRestResponse<TReturn>> MakeRequest<TReturn, TBody>(
            string actionName, 
            TBody body, 
            Dictionary<string, string> parameters, 
            int? impersonateUser)
        {
            return await MakeRequest<TReturn>();
        }

        private async Task<LaDanseRestResponse<TReturn>> MakeRequest<TReturn>()
        {
            using (var httpClient = new HttpClient())
            {
                PrepareHttpClient(httpClient);
                
                _logger.LogDebug("Making PostAsJsonAsync call");

                var responseMsg = await httpClient.GetStringAsync(CreateEndpointUri());

                _logger.LogDebug(responseMsg);

                if (true)
                {
                    return new LaDanseRestResponse<TReturn>(
                        JsonConvert.DeserializeObject<TReturn>(responseMsg));    
                }
                else
                {
                    return await CreateException<TReturn>(new Exception("ThrowAsync Test 2"));
                }
            }
        }
        
        private Uri CreateEndpointUri()
        {
            return new Uri(_config["ladanse:api:baseUrl"] + "/api/events/");
        }
        
        private void PrepareHttpClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add(
                "X-LADANSE-DISCORD-AUTH-DIGEST",
                CreateAuthenticationDigest());
            
            /*
            httpClient.DefaultRequestHeaders.Add(
                "X-LADANSE-DISCORD-IMPERSONATION",
                "<insert access token here>");
            */
        }

        private static Task<LaDanseRestResponse<TReturn>> CreateException<TReturn>(Exception e)
        {
            var tcs = new TaskCompletionSource<LaDanseRestResponse<TReturn>>();
                    
            ThreadPool.QueueUserWorkItem(_ => tcs.SetException(e));
                    
            return tcs.Task;
        }

        private string CreateAuthenticationDigest()
        {
            var ladanseApiSecret = _config["ladanse:api:secret"];

            var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            
            var random = Random("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 16);

            var toHash = random + ":" + currentTimestamp + ":" + ladanseApiSecret;
            
            var bytes = Encoding.UTF8.GetBytes(toHash);
            
            var sha256Managed = new SHA256Managed();
            
            var hash = sha256Managed.ComputeHash(bytes);
            
            var hashString = hash.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
            
            return random + ":" + currentTimestamp + ":" + hashString;
        }
        
        private static string Random(string chars, int length = 8)
        {
            var randomString = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
                randomString.Append(chars[random.Next(chars.Length)]);

            return randomString.ToString();
        }
    }
}
