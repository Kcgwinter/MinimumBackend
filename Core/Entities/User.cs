using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [NotMapped]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public bool IsActive { get; set; } = true;

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpires { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        public string? EmailConfirmationToken { get; set; }

        public DateTime? EmailConfirmationTokenExpires { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
