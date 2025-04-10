using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace FinanceSystem.API.Models
{
    public enum CategoryType
    {
        Income,
        Expense
    }

    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public CategoryType Type { get; set; }

        public bool Fixed { get; set; } = false;

        //FK e relacionamento
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}