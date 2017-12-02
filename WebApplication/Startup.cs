using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LaDanseDiscordBot.Persistence;
using LaDanseDiscordBot.Services;
using LaDanseRestTransport;
using LaDanseServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<DiscordBotContext>(
                options => options.UseMySql(Configuration.GetSection("database")["connection"])
            );

            services.AddSingleton<DbContextFactory>();

            services.AddSingleton(
                new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000 // Tell Discord.Net to cache 1000 messages per channel
                }));

            services.AddSingleton(new CommandService(
                new CommandServiceConfig // Add the command service to the service provider
                {
                    DefaultRunMode = RunMode.Async, // Force all commands to run async
                    LogLevel = LogSeverity.Verbose
                }));

            services
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration);

            services
                .AddSingleton<LaDanseUrlBuilder>()
                .AddSingleton<LaDanseRestClient>()
                .AddSingleton<EventService>();

            foreach (var service in services)
            {
                if (service.ServiceType.FullName.Contains("Context"))
                {
                    Console.WriteLine($"Service: {service.ServiceType.FullName}\n      Lifetime: {service.Lifetime}\n      Instance: {service.ImplementationType?.FullName}");
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Console.WriteLine("Configure");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Initialize the logging service, startup service, and command handler
            app.ApplicationServices.GetRequiredService<LoggingService>();
            app.ApplicationServices.GetRequiredService<StartupService>().StartAsync().Wait();
            app.ApplicationServices.GetRequiredService<CommandHandler>();

            app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program))
                .LogInformation("Starting La Danse Discord Bot");

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
