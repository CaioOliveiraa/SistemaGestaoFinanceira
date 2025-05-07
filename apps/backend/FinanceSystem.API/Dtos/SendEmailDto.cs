using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.API.Dtos
{
    public class SendEmailDto
    {
        [Required, EmailAddress]
        public string To { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;
    }
}
