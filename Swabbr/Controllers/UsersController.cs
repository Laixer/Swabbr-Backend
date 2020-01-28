using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public UsersController(
            IUserRepository userRepository,
            IFollowRequestRepository followRequestRepository,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IReactionRepository reactionRepository,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _userRepository = userRepository;
            _followRequestRepository = followRequestRepository;
            _vlogRepository = vlogRepository;
            _vlogLikeRepository = vlogLikeRepository;
            _reactionRepository = reactionRepository;
            _userManager = userManager;
        }

        private async Task<UserOutputModel> GetUserOutputAsync(User u)
        {
            UserOutputModel output = u;
            output.TotalVlogs = await _vlogRepository.GetVlogCountForUserAsync(output.UserId);
            output.TotalFollowers = await _followRequestRepository.GetFollowerCountAsync(output.UserId);
            output.TotalFollowing = await _followRequestRepository.GetFollowingCountAsync(output.UserId);
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
                User user = await _userRepository.GetByIdAsync(userId);
                UserOutputModel output = await GetUserOutputAsync(user);
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "User could not be found.")
                    );
            }
        }

        // TODO remove unused parameters
        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="query">Search query.</param>
        [HttpGet("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> SearchAsync([FromQuery]string query)
        {
            //TODO Search not yet implemented, temporary solution
            var results = await _userRepository.SearchAsync(query, 0, 0);

            var filteredResults = results.Where(
                x =>
                x.FirstName.ToUpperInvariant().Contains(query.ToUpperInvariant()) ||
                x.LastName.ToUpperInvariant().Contains(query.ToUpperInvariant()) ||
                x.Nickname.ToUpperInvariant().Contains(query.ToUpperInvariant())
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
            var userId = identityUser.UserId;
            var entity = await _userRepository.GetByIdAsync(userId);

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

            var userEntity = await _userRepository.GetByIdAsync(identityUser.UserId);

            //TODO Should Country/Gender/Birth Date etc. be allowed to be changed?
            // Update properties
            userEntity.FirstName = input.FirstName;
            userEntity.LastName = input.LastName;
            userEntity.Country = input.Country;
            userEntity.Gender = input.Gender;
            userEntity.Nickname = input.Nickname;
            userEntity.ProfileImageUrl = input.ProfileImageUrl;
            userEntity.Timezone = input.Timezone;
            userEntity.BirthDate = input.BirthDate;

            var updatedEntity = await _userRepository.UpdateAsync(input);

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
            //TODO Delete all user data? Or flag user as inactive? Not yet implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("{userId}/following")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> ListFollowingAsync([FromRoute] Guid userId)
        {
            var followRelationships = await _followRequestRepository.GetOutgoingForUserAsync(userId);
            var usersOutput = followRelationships
                .Where(x => x.Status == FollowRequestStatus.Accepted)
                .Select(async x =>
                {
                    var u = await _userRepository.GetByIdAsync(x.RequesterId);
                    UserOutputModel o = await GetUserOutputAsync(u);
                    return o;
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
            int totalViewsCount = -1;

            // Add up statistical data for each of the users' vlogs.
            foreach (Vlog v in userVlogs)
            {
                totalReactionsReceivedCount += await _reactionRepository.GetReactionCountForVlogAsync(v.VlogId);
                totalLikesReceivedCount += v.Likes.Count;

                // TODO: Add up received views for each vlog here: v
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
        /// Deletes the follow relationship from the authorized user to the specified user.
        /// </summary>
        [HttpDelete("{userId}/unfollow")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UnfollowAsync([FromRoute] Guid userId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                var followRequest = await _followRequestRepository.GetByUserIdAsync(userId, identityUser.UserId);
                // Delete the request
                await _followRequestRepository.DeleteAsync(followRequest);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Relationship could not be found.")
                    );
            }
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
                    var u = await _userRepository.GetByIdAsync(x.RequesterId);
                    UserOutputModel o = await GetUserOutputAsync(u);
                    return o;
                })
                .Select(t => t.Result);

            return Ok(usersOutput);
        }
    }
}