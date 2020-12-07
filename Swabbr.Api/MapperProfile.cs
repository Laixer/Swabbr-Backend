using AutoMapper;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;

namespace Swabbr.Api
{
    // TODO Correct?
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
            mapper.CreateMap<SwabbrUser, UserDto>().ReverseMap();
            mapper.CreateMap<SwabbrUserWithStats, UserWithStatsDto>();
            mapper.CreateMap<Vlog, VlogDto>().ReverseMap();
            mapper.CreateMap<Vlog, VlogWithSummaryDto>(); // TODO Why do we need this?
            mapper.CreateMap<VlogWithThumbnailDetails, VlogDto>()
                .IncludeMembers(src => src.Vlog);
            mapper.CreateMap<VlogWithThumbnailDetails, VlogWithSummaryDto>()
               .IncludeMembers(src => src.Vlog);
            mapper.CreateMap<VlogLike, VlogLikeDto>()
                .ForMember(dest => dest.UserId, o => o.MapFrom(src => src.Id.UserId))
                .ForMember(dest => dest.VlogId, o => o.MapFrom(src => src.Id.VlogId));
        }
    }
}
