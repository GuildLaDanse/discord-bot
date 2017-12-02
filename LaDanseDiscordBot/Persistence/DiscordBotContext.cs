using System;
using System.Collections.Generic;
using System.Text;
using LaDanseDiscordBot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaDanseDiscordBot.Persistence
{
    public class DiscordBotContext : DbContext
    {
        public DiscordBotContext(DbContextOptions<DiscordBotContext> options)
            : base(options)
        { }

        public DbSet<DiscordUser> DiscordUsers { get; set; }

        public DbSet<AuthSession> AuthSessions { get; set; }
        public DbSet<AccessTokenMapping> AccessTokenMappings { get; set; }
    }
}
