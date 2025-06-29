using AutoMapper;
using DataAccess.Entities;
using Models;

namespace DataAccess
{
    public class SystemMapping : Profile
    {
        public SystemMapping()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDto>();

            CreateMap<TodoItemDto, TodoItem>().ReverseMap();
            CreateMap<TodoItem, TodoItemDto>();

            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<Category, CategoryDto>();
        } 
    }
}
