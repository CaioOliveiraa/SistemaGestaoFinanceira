using System.ComponentModel.DataAnnotations;

namespace FinanceSystem.API.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set;} = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}