using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBotAPI.Mapping
{
    [Table("tbl_users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public int Id { get; set; }
        [Column("UserDiscordId")]
        public long DiscordId { get; set; }
        [Required]
        public long Points { get; set; }
        [Required]
        public long BetsExecuted { get; set; }
        [Required]
        public long BetsWon { get; set; }
        [Required]
        public long BetsLost { get; set; }
        public string Username { get; set; }
    }
}