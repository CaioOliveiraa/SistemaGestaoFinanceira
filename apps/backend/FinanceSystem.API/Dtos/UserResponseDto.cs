using FinanceSystem.API.Models;

namespace FinanceSystem.API.Dtos
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserType Type { get; set; }
    }
}