﻿using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Swabbr.Api.Authentication;
using Swabbr.Api.Utility;
using Swabbr.Api.ViewModels.Notifications.JsonWrappers;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// This contains information about the API types.
    /// TODO This exposes the core.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("types")]
    public class TypesController : ApiControllerBase
    {

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public TypesController() { }

        /// <summary>
        /// JSON wrapper around our notifications.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_wrapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(NotificationPayloadJsonWrapperModel))]
        public async Task<IActionResult> NotificationWrapper()
        {
            return Ok(new NotificationPayloadJsonWrapperModel());
        }

        /// <summary>
        /// JSON wrapper for the data part of clickaction <see cref="NotificationAction.FollowedProfileLive"/>.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_followed_profile_live")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ParametersFollowedProfileLive))]
        public async Task<IActionResult> NotificationDataFollowedProfileLive()
        {
            return Ok(new ParametersFollowedProfileLive());
        }

        /// <summary>
        /// JSON wrapper for the data part of clickaction <see cref="NotificationAction.FollowedProfileVlogPosted"/>.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_followed_profile_vlog_posted")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ParametersFollowedProfileVlogPosted))]
        public async Task<IActionResult> NotificationDataFollowedProfileVlogPosted()
        {
            return Ok(new ParametersFollowedProfileVlogPosted());
        }

        /// <summary>
        /// JSON wrapper for the data part of clickaction <see cref="NotificationAction.VlogRecordRequest"/>.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_vlog_record_request")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ParametersRecordVlog))]
        public async Task<IActionResult> NotificationDataVlogRecordRequest()
        {
            return Ok(new ParametersRecordVlog());
        }

        /// <summary>
        /// JSON wrapper for the data part of clickaction <see cref="NotificationAction.VlogGainedLikes"/>.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_vlog_gained_likes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ParametersVlogGainedLike))]
        public async Task<IActionResult> NotificationDataVlogGainedLikes()
        {
            return Ok(new ParametersVlogGainedLike());
        }

        /// <summary>
        /// JSON wrapper for the data part of clickaction <see cref="NotificationAction.VlogNewReaction"/>.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_vlog_new_reaction")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ParametersVlogNewReaction))]
        public async Task<IActionResult> NotificationDataVlogNewReaction()
        {
            return Ok(new ParametersVlogNewReaction());
        }

        /// <summary>
        /// Lists all possible notification click types.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_notification_click_types")]
        public async Task<IActionResult> ListNotificationClickTypes()
        {
            var result = new List<string>();
            foreach (var e in (NotificationAction[])Enum.GetValues(typeof(NotificationAction)))
            {
                result.Add(NotificationActionTranslator.Translate(e));
            }
            return Ok(result);
        }

    }

}
