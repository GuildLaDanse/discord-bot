using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LaDanseDiscordBot.Persistence
{
    public class DbContextFactory
    {
        private readonly IConfiguration _config;

        public DbContextFactory(IConfiguration config)
        {
            _config = config;
        }

        public DiscordBotContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscordBotContext>();
            optionsBuilder.UseMySql(_config.GetSection("database")["connection"]);
            //optionsBuilder.UseSqlite("Data Source=discordbot.db");

            return new DiscordBotContext(optionsBuilder.Options);
        }
    }
}
