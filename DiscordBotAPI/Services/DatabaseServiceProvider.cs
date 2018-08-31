namespace DiscordBotAPI.Models
{
    using System.Data.Entity;
    using DiscordBotAPI.Mapping;
    using DiscordBotAPI.Services;

    public class DatabaseServiceProvider : DbContext, IDatabaseServiceProvider
    {
        // Your context has been configured to use a 'DatabaseServiceProvider' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'DiscordBotAPI.Models.DatabaseServiceProvider' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DatabaseServiceProvider' 
        // connection string in the application configuration file.
        
        // Base string is the name of the configuration string inside the webconfig file. If the connection to database fails, make sure that string is correct.
        public DatabaseServiceProvider()
            : base("name=dbModel")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.
        // All mapped classes

        public virtual DbSet<User> Users { get; set; }

        // Returns current DBContext
        public virtual DbContext Context
        {
            get
            {
                return this;
            }
        }
    }
}