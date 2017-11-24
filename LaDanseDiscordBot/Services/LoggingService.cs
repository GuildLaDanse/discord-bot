using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot.Services
{
    public class LoggingService
    {
        private readonly ILogger _logger;

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingService(DiscordSocketClient discord, CommandService commands, ILogger<IDiscordClient> logger)
        {
            _logger = logger;

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            string logText =
                $"{msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";

            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(logText);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(logText);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(logText);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(logText);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(logText);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(logText);
                    break;
                default:
                    _logger.LogWarning(logText);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}