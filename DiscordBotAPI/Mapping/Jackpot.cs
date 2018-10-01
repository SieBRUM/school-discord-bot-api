using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TNSApi.Mapping
{
    public enum JackpotStatus
    {
        JackpotAlreadyExists,
        UserDoesntExist,
        UserNotEnoughPoints,
        JackpotEnded,
        InvalidPoints,
        UnknownError
    }

    [Table("tbl_jackpot")]
    public class Jackpot
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public long Points { get; set; }

        [NotMapped]
        public decimal WinChancePercentage { get; set; }
        [NotMapped]
        public long TotalPoints { get; set; }
        [NotMapped]
        public long DiscordId { get; set; }
        [NotMapped]
        public JackpotStatus Status { get; set; }
    }
}