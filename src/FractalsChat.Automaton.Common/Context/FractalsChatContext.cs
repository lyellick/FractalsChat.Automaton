using FractalsChat.Automaton.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System.Text;

namespace FractalsChat.Automaton.Common.Context
{
    public class FractalsChatContext : DbContext
    {
        public virtual DbSet<Network> Networks { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Bot> Bots { get; set; }

        public FractalsChatContext()
        {
        }

        public FractalsChatContext(DbContextOptions<FractalsChatContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
                builder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);

            builder.Entity<Network>(entity =>
            {
                entity.HasData(
                    new Network() { NetworkId = 1, Name = "Fractals Chat", Description = "Fractals Chat IRC Server", Domain = "irc.fractals.chat", Port = 6667 }
                );
            });

            builder.Entity<Channel>(entity =>
            {
                entity.HasData(
                    new Channel() { ChannelId = 1, Name = "#botplayground", Description = "Fractals Chat IRC Server", Created = DateTimeOffset.UtcNow }
                );
            });

            builder.Entity<Bot>(entity =>
            {
                entity.HasData(
                    new Bot() { BotId = 1, Nickname = "fluxbot", Description = "Bot Used Within the Fractals Chat IRC Network", Gecos = "Fluxbot v1.0 (fractals.chat)", 
                                Ident = "fluxbot", Password = "", Created = DateTimeOffset.UtcNow, BotGuid = Guid.NewGuid() }
                );
            });

            builder.Entity<Session>(entity =>
            {
                entity.HasData(
                    new Session() { SessionId = 1, NetworkId = 1, ChannelId = 1, BotId = 1, SessionGuid = Guid.NewGuid() }
                );
            });
        }
    }
}
