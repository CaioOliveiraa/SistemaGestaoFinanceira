using System.ComponentModel.DataAnnotations;
using FinanceSystem.API.Models;

namespace FinanceSystem.API.Dtos
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set;} = string.Empty;

        [Required]
        public CategoryType Type { get; set;}

        public bool Fixed { get; set; } = false;
    }
}