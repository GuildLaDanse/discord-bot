using System;
using System.Linq;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Persistence.Entities;
using LaDanseRestTransport;
using LaDanseServices.Dto.Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    public class ConnectController : Controller
    {
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;
        private readonly LaDanseRestClient _laDanseRestClient;
        private readonly ILogger _logger;
        private readonly DiscordBotContext _dbContext;
        
        public ConnectController(
            LaDanseUrlBuilder laDanseUrlBuilder, 
            LaDanseRestClient laDanseRestClient, 
            ILogger<ConnectController> logger,
            DiscordBotContext dbContext)
        {
            _laDanseUrlBuilder = laDanseUrlBuilder;
            _laDanseRestClient = laDanseRestClient;
            _logger = logger;
            _dbContext = dbContext;
        }
        
        public IActionResult Website(string authCode)
        {
            _logger.LogTrace("ConnectController - " + authCode);

            #region FetchAccessToken
            
            var discordSiteResponse = _laDanseRestClient
                .GetAsync<AccessTokenResponse>(_laDanseUrlBuilder.DiscordGrantUrl(authCode))
                .GetAwaiter().GetResult();

            if (!discordSiteResponse.IsSuccess)
            {
                _logger.LogTrace("Failed - GetAsync");
                return View("Failure");
            }

            var accessTokenResponse = discordSiteResponse.Body;
                
            #endregion
            
            #region Fetch AuthSession

            var authSessions = _dbContext.AuthSessions
                    .Include(a => a.DiscordUser)
                    .Where(a => a.Nonce == accessTokenResponse.Nonce)
                    .ToList();

            if (authSessions.Count != 1)
            {
                _logger.LogTrace("Failed - no matching AuthSession");
                return View("Failure");
            }

            var authSession = authSessions[0];

            Console.WriteLine("AuthSession.DiscordUser");
            Console.WriteLine(authSession.AuthSessionId);
            Console.WriteLine(authSession.Nonce);
            Console.WriteLine(authSession.State);
            Console.WriteLine(authSession.DiscordUser == null);
            
            #endregion

            #region CleanUpAccessTokenMappings

            var accessTokenMappings = _dbContext.AccessTokenMappings
                .Where(a => a.DiscordUser == authSession.DiscordUser)
                .ToList();
            
            foreach (var accessTokenMapping in accessTokenMappings)
            {
                if (accessTokenMapping.State == AccessTokenState.Active)
                    accessTokenMapping.State = AccessTokenState.Removed;
            }

            #endregion
            
            #region CreateAccessTokenMapping

            var newAccessTokenMapping = new AccessTokenMapping
            {
                AccessToken = accessTokenResponse.AccessToken,
                CreatedOn = 0,
                DiscordUser = authSession.DiscordUser,
                State = AccessTokenState.Active
            };

            _dbContext.AccessTokenMappings.Add(newAccessTokenMapping);

            authSession.State = AuthSessionState.Consumed;

            #endregion

            _dbContext.SaveChanges();
            
            return View("Success");
        }
    }
}