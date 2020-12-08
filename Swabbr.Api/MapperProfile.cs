using AutoMapper;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Entities;

namespace Swabbr.Api
{
    // TODO Move to some extension?
    /// <summary>
    ///     Contains our DTO mapping profile.
    /// </summary>
    internal static class MapperProfile
    {
        /// <summary>
        ///     Setup our DTO mapping profile.
        /// </summary>
        internal static void SetupProfile(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<FollowRequest, FollowRequestDto>()
                .ForMember(dest => dest.ReceiverId, o => o.MapFrom(src => src.Id.ReceiverId))
                .ForMember(dest => dest.RequesterId, o => o.MapFrom(src => src.Id.RequesterId));
            mapper.CreateMap<Reaction, ReactionDto>().ReverseMap();
            mapper.CreateMap<User, UserDto>();
            mapper.CreateMap<User, UserCompleteDto>().ReverseMap();
            mapper.CreateMap<UserWithStats, UserWithStatsDto>();
            mapper.CreateMap<Vlog, VlogDto>().ReverseMap();
            mapper.CreateMap<Vlog, VlogWithSummaryDto>();
            mapper.CreateMap<VlogLike, VlogLikeDto>()
                .ForMember(dest => dest.UserId, o => o.MapFrom(src => src.Id.UserId))
                .ForMember(dest => dest.VlogId, o => o.MapFrom(src => src.Id.VlogId));
        }
    }
}
