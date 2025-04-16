using System.ComponentModel.DataAnnotations;
using FinanceSystem.API.Models;

namespace FinanceSystem.API.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser acima de 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool Recurring { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}