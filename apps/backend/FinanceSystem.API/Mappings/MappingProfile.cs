using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using AutoMapper;

namespace FinanceSystem.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            CreateMap<CreateUserDto, User>(); // Quando eu pedir para converter um CreateUserDto em User, copie as propriedades com nomes compativeis
            CreateMap<User, UserResponseDto>();

            //Category
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            //Transaction
            CreateMap<TransactionDto, Transaction>();
            CreateMap<Transaction, TransactionDto>();
        }
    }
}