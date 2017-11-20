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

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
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
            
            if (msg.Author == _discord.CurrentUser) return;     // Ignore self when checking commands
            
            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)
                {
                    System.Console.WriteLine(result.ToString());

                    if (CommandError.UnknownCommand == result.Error)
                    {
                        await context.Channel.SendMessageAsync("Sorry, I don't know that command, try !help for a list of commands you can use.");
                    }
                    else if (CommandError.BadArgCount == result.Error)
                    {
                        await context.Channel.SendMessageAsync("Try !help for a list of commands and the parameters you can pass.");
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync("Oh my, something went wrong on my side! Shouts for help have gone out!");
                    }
                    
                    
                }
            }
        }
    }
}