using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace LaDanseDiscordBot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }
        
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            
            // Ignore self when checking commands
            if (msg.Author == _discord.CurrentUser) return;     

            var context = new SocketCommandContext(_discord, msg);     

            // Check if the message has a valid command prefix
            // A valid command prefix is either the prefixer characters or a direct reference of the bot user
            int argPos = 0;     
            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                // Execute the command
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     

                if (!result.IsSuccess)
                {
                    System.Console.WriteLine(result.ToString());

                    if (CommandError.UnknownCommand == result.Error)
                    {
                        await context.Channel.SendMessageAsync($"Sorry, I don't know that command, try {_config["prefix"]}help for a list of commands you can use.");
                    }
                    else if (CommandError.BadArgCount == result.Error)
                    {
                        await context.Channel.SendMessageAsync($"Try {_config["prefix"]}help for a list of commands and the parameters you can pass.");
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync("Oh my, something went wrong on my side :( Shouts for help have gone out!");
                    }
                }
            }
        }
    }
}