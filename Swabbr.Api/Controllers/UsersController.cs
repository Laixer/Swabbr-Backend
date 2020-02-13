using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to users.
    /// </summary>
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        // Repositories
        private readonly IFollowRequestRepository _followRequestRepository;

        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly IReactionRepository _reactionRepository;

        // Services
        private readonly IUserService _userService;

        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public UsersController(
            IFollowRequestRepository followRequestRepository,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IReactionRepository reactionRepository,
            IUserService userService,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _followRequestRepository = followRequestRepository;
            _vlogRepository = vlogRepository;
            _vlogLikeRepository = vlogLikeRepository;
            _reactionRepository = reactionRepository;

            _userService = userService;

            _userManager = userManager;
        }

        // TODO: Move to separate (ViewModel) service that parses the entity and retrieves statistics.
        private async Task<UserOutputModel> GetUserOutputAsync(User entity)
        {
            UserOutputModel output = UserOutputModel.Parse(entity);
            output.TotalVlogs = await _vlogRepository.GetVlogCountForUserAsync(output.Id);
            output.TotalFollowers = await _followRequestRepository.GetFollowerCountAsync(output.Id);
            output.TotalFollowing = await _followRequestRepository.GetFollowingCountAsync(output.Id);
            return output;
        }

        /// <summary>
        /// Get information about a single user.
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid userId)
        {
            try
            {
                //TODO: Use Services
                User user = await _userService.GetAsync(userId);
                UserOutputModel output = await GetUserOutputAsync(user);
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "User could not be found.")
                );
            }
        }

        //TODO remove unused parameters
        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="query">Search query.</param>
        [HttpGet("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> SearchAsync([FromQuery]string query)
        {
            //TODO: Search not yet implemented, temporary solution, full scan
            var results = await _userService.SearchAsync(query, 0, 0);

            var filteredResults = results.Where(
                x =>
                x.FirstName.ToUpperInvariant().Contains(query.ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase) ||
                x.LastName.ToUpperInvariant().Contains(query.ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase) ||
                x.Nickname.ToUpperInvariant().Contains(query.ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase)
                ).Select(async x =>
                {
                    UserOutputModel o = await GetUserOutputAsync(x);
                    return o;
                })
                .Select(t => t.Result);

            return Ok(filteredResults);
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> SelfAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var entity = await _userService.GetAsync(identityUser.UserId);

            UserOutputModel output = await GetUserOutputAsync(entity);

            return Ok(output);
        }

        /// <summary>
        /// Updates the authenticated user.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateInputModel input)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var userEntity = await _userService.GetAsync(identityUser.UserId);

            //TODO: Should Country/Gender/Birth Date etc. be allowed to be changed?
            // Update properties
            userEntity.FirstName = input.FirstName;
            userEntity.LastName = input.LastName;
            userEntity.Country = input.Country;
            userEntity.Gender = input.Gender;
            userEntity.Nickname = input.Nickname;
            userEntity.ProfileImageUrl = input.ProfileImageUrl;
            userEntity.Timezone = input.Timezone;
            userEntity.BirthDate = input.BirthDate;

            var updatedEntity = await _userService.UpdateAsync(userEntity);

            UserOutputModel output = await GetUserOutputAsync(updatedEntity);

            return Ok(output);
        }

        /// <summary>
        /// Deletes the account of the authenticated user.
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync()
        {
            //TODO: Delete all user data? Or flag user as inactive? Not yet implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("{userId}/following")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> ListFollowingAsync([FromRoute] Guid userId)
        {
            var usersOutput =
                (await _followRequestRepository.GetOutgoingForUserAsync(userId))
                .Where(request => request.Status == FollowRequestStatus.Accepted)
                .Select(async request =>
                {
                    var user = await _userService.GetAsync(request.RequesterId);
                    UserOutputModel output = await GetUserOutputAsync(user);
                    return output;
                })
                .Select(t => t.Result);

            return Ok(usersOutput);
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("self/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsSelfAsync()
        {
            var userId = (await _userManager.GetUserAsync(User)).UserId;
            return await GetStatisticsAsync(userId);
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("{userId}/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] Guid userId)
        {
            var userVlogs = await _vlogRepository.GetVlogsByUserAsync(userId);

            int totalReactionsReceivedCount = 0;
            int totalLikesReceivedCount = 0;

            //TODO: Total views counter should be incremented in the for each loop for vlogs below
            int totalViewsCount = int.MinValue;

            // Add up statistical data for each of the users' vlogs.
            foreach (var vlog in userVlogs)
            {
                totalReactionsReceivedCount += await _reactionRepository.GetReactionCountForVlogAsync(vlog.Id);
                totalLikesReceivedCount += vlog.Likes.Count();

                //TODO: Add up received views for each vlog here: v
                //// totalViews += ?
            }

            return Ok(new UserStatisticsOutputModel
            {
                TotalFollowers = await _followRequestRepository.GetFollowerCountAsync(userId),
                TotalFollowing = await _followRequestRepository.GetFollowingCountAsync(userId),
                TotalVlogs = await _vlogRepository.GetVlogCountForUserAsync(userId),
                TotalReactionsGiven = await _reactionRepository.GetGivenReactionCountForUserAsync(userId),
                TotalLikes = totalLikesReceivedCount,
                TotalReactionsReceived = totalReactionsReceivedCount,
                TotalViews = totalViewsCount
            });
        }

        /// <summary>
        /// Get the followers of a single user.
        /// </summary>
        [HttpGet("{userId}/followers")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> ListFollowersAsync([FromRoute] Guid userId)
        {
            var followRelationships = await _followRequestRepository.GetIncomingForUserAsync(userId);

            var usersOutput = followRelationships
                .Where(x => x.Status == FollowRequestStatus.Accepted)
                .Select(async x =>
                {
                    var u = await _userService.GetAsync(x.RequesterId);
                    UserOutputModel o = await GetUserOutputAsync(u);
                    return o;
                })
                .Select(t => t.Result);

            return Ok(usersOutput);
        }
    }
}