using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LaDanseDiscordBot.Services;
using LaDanseRestAPI.Services;
using LaDanseSiteConnector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LaDanseDiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
        
        private IConfigurationRoot _config;

        public async Task StartAsync()
        {
            var builder = new ConfigurationBuilder()    // Begin building the configuration file
                .SetBasePath(AppContext.BaseDirectory)  // Specify the location of the config
                .AddJsonFile("configuration.json");     // Add the configuration file
            _config = builder.Build();                  // Build the configuration file

            var services = new ServiceCollection()      // Begin building the service provider
                
                // Add the discord client to the service provider
                .AddSingleton(
                    new DiscordSocketClient(new DiscordSocketConfig     
                    {
                        LogLevel = LogSeverity.Verbose,
                        MessageCacheSize = 1000     // Tell Discord.Net to cache 1000 messages per channel
                    })
                )
                
                // Add the command service to the service provider
                .AddSingleton(new CommandService(new CommandServiceConfig     // Add the command service to the service provider
                {
                    DefaultRunMode = RunMode.Async,     // Force all commands to run async
                    LogLevel = LogSeverity.Verbose
                }))

                .AddSingleton(new LoggerFactory()
                    .AddConsole(LogLevel.Trace)
                    .AddDebug())
                .AddLogging()

                // Add remaining services to the provider
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()     
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()             
                .AddSingleton(_config)

                // Add La Danse services
                .AddSingleton<LaDanseUrlBuilder>()
                .AddSingleton<LaDanseRestClient>()
                .AddSingleton<EventService>();

            var provider = services.BuildServiceProvider();     // Create the service provider

            // Initialize the logging service, startup service, and command handler
            provider.GetRequiredService<LoggingService>();      
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandler>();

            provider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program))
                .LogInformation("Starting La Danse Discord Bot");

            //Console.WriteLine("Starting La Danse Discord Bot");
            
            await Task.Delay(-1);     // Prevent the application from closing
        }
    }
}