using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        DbContext Context { get; }
    }
}
