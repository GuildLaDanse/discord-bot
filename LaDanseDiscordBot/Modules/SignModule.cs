using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotCommon;
using Discord.Commands;
using LaDanseDiscordBot.Services;
using LaDanseRestTransport;
using LaDanseServices.Dto.Event;
using LaDanseServices.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot.Modules
{
    struct SignUpRequest
    {
        public string signUpState;
        public string eventDate;
        public List<string> roles;
    }
    
    [Name("Sign")]
    public class SignModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly EventService _eventService;
        private readonly LaDanseUrlBuilder _laDanseUrlBuilder;
        private readonly LaDanseCommandContextFactory _laDanseCmdContextFactory;

        private readonly ILogger _logger;

        public SignModule(
            IConfiguration config, 
            EventService eventService, 
            LaDanseUrlBuilder laDanseUrlBuilder,
            LaDanseCommandContextFactory laDanseCmdContextFactory,
            ILogger<SignModule> logger)
        {
            _config = config;
            _eventService = eventService;
            _laDanseUrlBuilder = laDanseUrlBuilder;
            _laDanseCmdContextFactory = laDanseCmdContextFactory;
            _logger = logger;
        }
        
        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign([Remainder] string text)
        {
            var laDanseCmdContext = await _laDanseCmdContextFactory.CreateContext(Context);

            await Sign(laDanseCmdContext, text);
        }

        [Command("sign")]
        [Summary("Sign up for a raid")]
        public async Task Sign()
        {
            var helpUrl = _laDanseUrlBuilder.GetDiscordHelpUrl();
            
            string resultStr = null;

            resultStr += $"I would love to sign you up, {Context.User.Username}, " +
                         $"but you need to give me more information ...\n\n" +
                         $"Try something like '!sign me up for 17/02 as healer and dps'";

            await ReplyAsync(resultStr);
        }

        private async Task Sign(LaDanseCommandContext laDanseCmdContext, string text)
        {
            switch (laDanseCmdContext.UserState)
            {
                case LaDanseCommandContext.DiscordUserState.New:
                    await ReplyAsync($"Sorry {Context.User.Username}, I would love to sign you up but I need to get " +
                                     $"to know you first. \n\n" +
                                     $"Say !hello to me so we can get to know each other better.");
                    break;
                case LaDanseCommandContext.DiscordUserState.Unknown:
                    await ReplyAsync($"Sorry {Context.User.Username}, I would love to sign you up but I need to get " +
                                     $"to know you first.\n\n" +
                                     $"Say !hello to me so we can get to know each other better.");
                    break;
                case LaDanseCommandContext.DiscordUserState.Forgotten:
                    await ReplyAsync($"Sorry {Context.User.Username}, I would love to sign you up but there seems to be a " +
                                     $"problem: I can remember you from the past but my memories are vague. \n\n" +
                                     $"Say !hello to me and we can perhaps rekindle our friendship?");
                    break;
                case LaDanseCommandContext.DiscordUserState.Known:
                    await SignKnownUser(laDanseCmdContext, text);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private async Task SignKnownUser(LaDanseCommandContext laDanseCmdContext, string text)
        {
            var signUpRequest = GetStructuredSignUpRequest(text);

            if (signUpRequest == null)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, I could not understand your sign up request. " +
                                 $"Type !help if you are unsure on how to sign up.");

                return;
            }
            
            await ReplyAsync($"Thank you {Context.User.Username}, {SignUpRequestToString(signUpRequest.Value)}");
            
            /*
             * Parse sign up type
             *
             * Find date
             *
             * Find roles
             *
             * Find event on date
             *
             * See if user is already signed up
             *     Fetch profile for account id
             *     Compare with sign up list for matched event
             *
             * Not sign up?
             *     Create new sign up
             *
             * Signed up?
             *     Update existing sign up
             */
        }

        private SignUpRequest? GetStructuredSignUpRequest(string text)
        {
            var lowerText = text.ToLower();
            
            var signUpState = GetSignUpState(lowerText);

            if (signUpState == null)
            {
                return null;
            }

            var eventDate = GetEventDate(lowerText);

            if (eventDate == null)
            {
                return null;
            }
            
            var roles = GetSignUpRoles(lowerText);
            
            if (roles == null && signUpState != SignUpType.Absence)
            {
                return null;
            }
            
            return new SignUpRequest
            {
                signUpState = signUpState,
                eventDate  = eventDate,
                roles = roles
                    
            };
        }

        private string GetSignUpState(string text)
        {
            var myRegex = new Regex(@"((?:will come)|(?:might come)|(?:absence)|(?:absent))");
            
            var matches = myRegex.Matches(text);

            if (matches.Count != 1)
            {
                _logger.LogDebug("Did not match any sign up state");

                return null;
            }
            
            var match = matches[0];

            var signUpState = match.Groups[1];

            switch (signUpState.Value)
            {
                case "will come":
                    return SignUpType.WillCome;
                case "might come":
                    return SignUpType.MightCome;
                case "absence":
                    return SignUpType.Absence;
                case "absent":
                    return SignUpType.Absence;
                default:
                    _logger.LogDebug($"Unknown sign up state found: '{signUpState.Value}'");
                    return null;
            }
        }
        
        private string GetEventDate(string text)
        {
            var myRegex = new Regex(@"(\d{1,2}/\d{1,2})");
            
            var matches = myRegex.Matches(text);
            
            if (matches.Count != 1)
            {
                _logger.LogDebug("Did not match event date or too many dates given");

                return null;
            }
            
            var match = matches[0];

            return match.Groups[1].Value;
        }

        private List<string> GetSignUpRoles(string text)
        {
            var myRegex = new Regex(@"((?:tank)|(?:healer)|(?:dps))");
            
            var matches = myRegex.Matches(text);

            var roles = new List<string>();

            var tankCount = 0;
            var healerCount = 0;
            var dpsCount = 0;

            for(var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                
                var roleMatch = match.Groups[1];

                switch (roleMatch.Value)
                {
                    case "tank":
                        tankCount++;
                        break;
                    case "healer":
                        healerCount++;
                        break;
                    case "dps":
                        dpsCount++;
                        break;
                    default:
                        _logger.LogDebug($"Unknown role found: '{roleMatch.Value}'");
                        return null;
                }
            }

            if (tankCount > 1 || healerCount > 1 || dpsCount > 1)
            {
                _logger.LogDebug($"Too many roles found");
                
                return null;
            }
            
            if (tankCount == 1)
                roles.Add(RoleType.Tank);
            
            if (healerCount == 1)
                roles.Add(RoleType.Healer);
            
            if (dpsCount == 1)
                roles.Add(RoleType.Dps);
            
            return roles;
        }

        private string SignUpRequestToString(SignUpRequest signUpRequest)
        {
            var resultStr = "";

            resultStr += signUpRequest.signUpState;
            resultStr += " / ";
            resultStr += signUpRequest.eventDate;
            resultStr += " / ";

            foreach (var role in signUpRequest.roles)
            {
                resultStr += role;
                resultStr += " ";
            }
            
            return resultStr;
        }
    }
}