﻿using AutoMapper;
using Swabbr.Api.Authentication;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Api
{
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
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            mapper.CreateMap<FollowRequest, FollowRequestDto>()
                .ForMember(dest => dest.ReceiverId, o => o.MapFrom(src => src.Id.ReceiverId))
                .ForMember(dest => dest.RequesterId, o => o.MapFrom(src => src.Id.RequesterId));
            mapper.CreateMap<Reaction, ReactionDto>().ReverseMap();
            mapper.CreateMap<TokenWrapper, TokenWrapperDto>();
            mapper.CreateMap<UploadWrapper, UploadWrapperDto>();
            mapper.CreateMap<User, UserDto>();
            mapper.CreateMap<User, UserCompleteDto>();
            mapper.CreateMap<UserWithStats, UserWithStatsDto>();
            mapper.CreateMap<Vlog, VlogDto>().ReverseMap();
            mapper.CreateMap<VlogLike, VlogLikeDto>()
                .ForMember(dest => dest.UserId, o => o.MapFrom(src => src.Id.UserId))
                .ForMember(dest => dest.VlogId, o => o.MapFrom(src => src.Id.VlogId));
            mapper.CreateMap<VlogLikeSummary, VlogLikeSummaryDto>();
        }
    }
}
