using AutoMapper;
using Swabbr.Api.ViewModels;
using Swabbr.Infrastructure.Data.Entities;

namespace Swabbr.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRegisterInputModel, UserEntity>();
            CreateMap<UserUpdateInputModel, UserEntity>();
        }
    }
}
