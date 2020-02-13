﻿using Newtonsoft.Json;
using Swabbr.Core.Entities;
using System;

namespace Swabbr.Api.ViewModels
{
    public class ReactionOutputModel
    {
        /// <summary>
        /// Id of the reaction.
        /// </summary>
        [JsonProperty("reactionId")]
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the user by whom this reaction was created.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// The moment at which the reaction was posted.
        /// </summary>
        [JsonProperty("datePosted")]
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Metadata from the Media Service
        /// </summary>
        [JsonProperty("mediaServiceData")]
        public object MediaServiceData { get; set; }

        public static implicit operator ReactionOutputModel(Reaction entity)
        {
            return new ReactionOutputModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                VlogId = entity.VlogId,
                DatePosted = entity.DatePosted,
                IsPrivate = entity.IsPrivate,
                MediaServiceData = entity.MediaServiceData
            };
        }
    }
}