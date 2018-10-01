using DiscordBotAPI.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TNSApi.Mapping
{
    [Table("tbl_coinflip")]
    public class Coinflip
    {
        [Key]
        public int Id { get; set; }
        public int ChallengerId { get; set; }
        public int EnemyId { get; set; }
        public long Points { get; set; }
        public int Side { get; set; }

        [NotMapped]
        public User Challenger { get; set; }
        [NotMapped]
        public User Enemy { get; set; }
        [NotMapped]
        public CoinflipVsResults Result { get; set; }
    }
}