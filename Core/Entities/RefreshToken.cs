using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [MaxLength(50)]
        public string RefreshTokenHash { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime Expires { get; set; }

        public User User { get; set; }
    }
}
