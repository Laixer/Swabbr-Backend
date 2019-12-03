using AutoMapper;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;

namespace Swabbr.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRegisterInput, User>();
            CreateMap<UserUpdateInput, User>();
        }
    }
}