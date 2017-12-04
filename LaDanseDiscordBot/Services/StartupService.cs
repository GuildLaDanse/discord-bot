using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace LaDanseDiscordBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        public StartupService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config)
        {
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            var discordToken = _config["discord:token"];
            
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the configuration file found in the start project root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await _discord.StartAsync();                                // Connect to the websocket

            // search for and add modules found in the assembly that hosts the type "StartupService" (current class)
            await _commands.AddModulesAsync(Assembly.GetAssembly(typeof(StartupService))); 
        }
    }
}