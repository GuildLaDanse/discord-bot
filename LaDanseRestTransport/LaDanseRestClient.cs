using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotCommon;
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

        public async Task<LaDanseRestResponse<TReturn>> GetAsync<TReturn>(string url, string accessToken = null)
        {
            using (var httpClient = new HttpClient())
            {
                PrepareHttpClient(httpClient, accessToken);

                var responseMsg = await httpClient.GetAsync(url);
                
                var content = await responseMsg.Content.ReadAsStringAsync();

                if (responseMsg.IsSuccessStatusCode)
                {
                    Console.WriteLine(content);
                    
                    return new LaDanseRestResponse<TReturn>(
                        JsonConvert.DeserializeObject<TReturn>(content)); 
                }
                else
                {
                    Console.WriteLine(content);
                    
                    return new LaDanseRestResponse<TReturn>(
                        JsonConvert.DeserializeObject<ErrorResponse>(content));
                }
            }
        }

        public async Task<LaDanseRestResponse<TReturn>> MakeRequest<TReturn, TBody>(
            string url, 
            TBody body,
            string accessToken = null)
        {
            throw new Exception("Not implemented");
        }
        
        private void PrepareHttpClient(HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add(
                "X-LADANSE-DISCORD-AUTH-DIGEST",
                CreateAuthenticationDigest());

            if (accessToken != null)
            {
                httpClient.DefaultRequestHeaders.Add(
                    "X-LADANSE-DISCORD-IMPERSONATION",
                    accessToken);
            }
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

            var random = RandomStringUtils.Random(16);

            var toHash = random + ":" + currentTimestamp + ":" + ladanseApiSecret;
            
            var bytes = Encoding.UTF8.GetBytes(toHash);
            
            var sha256Managed = new SHA256Managed();
            
            var hash = sha256Managed.ComputeHash(bytes);
            
            var hashString = hash.Aggregate(string.Empty, (current, x) => current + string.Format("{0:x2}", x));
            
            return random + ":" + currentTimestamp + ":" + hashString;
        }
    }
}
