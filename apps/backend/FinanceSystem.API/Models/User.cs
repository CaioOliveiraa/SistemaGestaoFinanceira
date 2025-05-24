using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.API.Models
{
    public enum UserType
    {
        Admin,
        Common
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserType Type { get; set; } = UserType.Common;

        public string? OtpSecret { get; set; }
        public string? OAuthProvider { get; set; }
        public string? OauthId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //Relacionamentos
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}