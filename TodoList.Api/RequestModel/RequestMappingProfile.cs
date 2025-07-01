using AutoMapper;
using DataAccess.Entities;
using Models;
using TodoList.Api.RequestModel.Todo;

namespace TodoList.Api.RequestModel
{
    public class RequestMappingProfile : Profile
    {
        public RequestMappingProfile() {
            CreateMap<TodoItemDto, TodoRequestModel>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>();

        }
    }
}
