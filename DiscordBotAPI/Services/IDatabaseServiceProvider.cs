using System.Data.Entity;
using DiscordBotAPI.Mapping;
using TNSApi.Mapping;

namespace DiscordBotAPI.Services
{
    /// <summary>
    /// Interface of DatabaseServiceProvider.
    /// Used for dependency injection
    /// </summary>
    public interface IDatabaseServiceProvider
    {
        DbSet<User> Users { get; set; }
        DbSet<Coinflip> Coinflips { get; set; }
        DbSet<Jackpot> Jackpot { get; set; }

        DbContext Context { get; }
    }
}
