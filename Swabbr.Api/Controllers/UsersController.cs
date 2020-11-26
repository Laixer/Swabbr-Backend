using Swabbr.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.Parsing;
using Swabbr.Api.ViewModels.Enums;
using Swabbr.Api.ViewModels.User;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests related to users. This is NOT used for
    ///     processing any requests regarding the identity side of the users.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UsersController(IUserService userService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(UsersController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Get information about a single <see cref="SwabbrUserWithStats"/>.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="OkObjectResult"/> or <see cref="NotFoundObjectResult"/></returns>
        [HttpGet("{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserWithStatsOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid userId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                var user = await _userService.GetWithStatisticsAsync(userId).ConfigureAwait(false);
                return Ok(MapperUser.Map(user));
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return NotFound(this.Error(ErrorCodes.EntityNotFound, "User could not be found."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get user"));
            }
        }

        /// <summary>
        /// Search for <see cref="SwabbrUserWithStats"/> in our data store.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="page">Page number</param>
        /// <param name="itemsPerPage">Items per page display</param>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpGet("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserWithStatsOutputModel>))]
        public async Task<IActionResult> SearchAsync([FromQuery]string query, [FromQuery]uint page = 1, [FromQuery]uint itemsPerPage = 50)
        {
            try
            {
                if (query.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Query string can't be null or empty")); }
                if (page < 1) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Page number must be greater than one")); }
                if (itemsPerPage < 1) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Items per page must be greater than one")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                // TODO Navigation will be cleaned up
                var users = await _userService.SearchAsync(query, new Navigation { Limit =  itemsPerPage, Offset = page - 1 }).ToListAsync();
                return Ok(users.Select(x => MapperUser.Map(x)));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not search for users"));
            }
        }

        /// <summary>
        /// Get information about the authenticated <see cref="SwabbrUserWithStats"/>.
        /// </summary>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserWithStatsOutputModel))]
        public async Task<IActionResult> SelfAsync()
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var userWithStats = await _userService.GetAsync(identityUser.Id).ConfigureAwait(false);
                return Ok(MapperUser.Map(userWithStats));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get information about self"));
            }
        }

        // TODO Is this the right location for this behaviour? --> IUserService?
        // TODO What you can update where is very inconsistent and random.
        //      Reconsider the settings anyways, the entire chaos exists because
        //      of the original UserSettings concept. Maybe remove this entirely?
        /// <summary>
        ///     Updates the authenticated user.
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserWithStatsOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateInputModel input)
        {
            try
            {
                if (input == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is invalid")); }

                // Act.
                var userId = (await _userManager.GetUserAsync(User).ConfigureAwait(false)).Id;

                // Act.
                var user = await _userService.GetAsync(userId);

                // Only map changed properties.
                user.BirthDate = input.BirthDate ?? user.BirthDate;
                user.Country = input.Country ?? user.Country;
                user.FirstName = input.FirstName ?? user.FirstName;
                user.Gender = MapperEnum.Map(input.Gender) ?? user.Gender;
                user.LastName = input.LastName ?? user.LastName;
                user.Nickname = input.Nickname ?? user.Nickname;
                user.ProfileImageBase64Encoded = input.ProfileImageBase64Encoded ?? user.ProfileImageBase64Encoded;

                // Perform updates and get the result.
                await _userService.UpdateAsync(user);
                var updatedUser = await _userService.GetAsync(userId).ConfigureAwait(false);

                // Map.
                var result = MapperUser.Map(updatedUser);

                // Return.
                return Ok(result);
            }
            catch (InvalidProfileImageStringException)
            {
                return BadRequest(this.Error(ErrorCodes.InvalidInput, "Profile image is invalid or not properly base64 encoded"));
            }
            catch (NicknameExistsException)
            {
                return Conflict(this.Error(ErrorCodes.EntityAlreadyExists, "Nickname is taken"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update user"));
            }
        }

        // TODO This does nothing with non-existent user ids
        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("{userId}/following")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowingOutputModel))]
        public async Task<IActionResult> ListFollowingAsync([FromRoute] Guid userId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                return Ok(new FollowingOutputModel
                {
                    Following = (await _userService.GetFollowingAsync(userId, Navigation.Default).ToListAsync()
                        .ConfigureAwait(false))
                        .Select(x => MapperUser.Map(x))
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not list users following"));
            }
        }

        // TODO This does nothing with non-existent user ids
        /// <summary>
        /// Get the followers of a single <see cref="SwabbrUser"/>.
        /// </summary>
        [HttpGet("{userId}/followers")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowersOutputModel))]
        public async Task<IActionResult> ListFollowersAsync([FromRoute] Guid userId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                return Ok(new FollowersOutputModel
                {
                    Followers = (await _userService.GetFollowersAsync(userId, Navigation.Default).ToListAsync()
                        .ConfigureAwait(false))
                        .Select(x => MapperUser.Map(x))
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get user followers"));
            }
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("{userId}/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] Guid userId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                return Ok(MapperUser.MapToStatistics(await _userService.GetWithStatisticsAsync(userId).ConfigureAwait(false)));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get statistics for user"));
            }
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("self/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsSelfAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                user.Id.ThrowIfNullOrEmpty();

                return await GetStatisticsAsync(user.Id).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get statistics for self"));
            }
        }
    }
}
