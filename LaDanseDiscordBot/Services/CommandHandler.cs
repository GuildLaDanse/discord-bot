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
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provider;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config,
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
            var argPos = 0;     
            if (msg.HasStringPrefix(_config["discord:prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                // Execute the command
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ToString());

                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand:
                            await context.Channel.SendMessageAsync($"Sorry, I don't know that command, try {_config["prefix"]}help for a list of commands you can use.");
                            break;
                        
                        case CommandError.BadArgCount:
                            await context.Channel.SendMessageAsync($"Try {_config["prefix"]}help for help on the commands I understand and how to use them.");
                            break;
                        
                        case CommandError.ParseFailed:
                        case CommandError.ObjectNotFound:
                        case CommandError.MultipleMatches:
                        case CommandError.UnmetPrecondition:
                        case CommandError.Exception:
                        case CommandError.Unsuccessful:
                        case null:
                            await context.Channel.SendMessageAsync("Oh my, something went wrong on my side :( Shouts for help have gone out!");
                            break;
                            
                        default:
                            await context.Channel.SendMessageAsync("Oh my, something went wrong on my side :( Shouts for help have gone out!");
                            break;
                    }
                }
            }
        }
    }
}