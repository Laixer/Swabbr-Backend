using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
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

        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUserWithStatsRepository _userWithStatsRepository;
        private readonly IUserService _userService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UsersController(IFollowRequestRepository followRequestRepository,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IReactionRepository reactionRepository,
            IUserWithStatsRepository userWithStatsRepository,
            IUserService userService,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _followRequestRepository = followRequestRepository ?? throw new ArgumentNullException(nameof(followRequestRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _userWithStatsRepository = userWithStatsRepository ?? throw new ArgumentNullException(nameof(userWithStatsRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Get information about a single <see cref="SwabbrUserWithStats"/>.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="OkObjectResult"/> or <see cref="NotFoundObjectResult"/></returns>
        [HttpGet("{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid userId)
        {
            try
            {
                var user = await _userWithStatsRepository.GetAsync(userId).ConfigureAwait(false);
                return Ok(MapperUser.Map(user));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(this.Error(ErrorCodes.EntityNotFound, "User could not be found."));
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> SearchAsync([FromQuery]string query, [FromQuery]int page = 1, [FromQuery]int itemsPerPage = 50)
        {
            query.ThrowIfNullOrEmpty();
            if (page < 1) { throw new ArgumentOutOfRangeException("Page number must be greater than one"); }
            if (itemsPerPage < 1) { throw new ArgumentOutOfRangeException("Items per page must be greater than one"); }

            var users = await _userWithStatsRepository.SearchAsync(query, page, itemsPerPage).ConfigureAwait(false);
            return Ok(users.Select(x => MapperUser.Map(x)));
        }

        /// <summary>
        /// Get information about the authenticated <see cref="SwabbrUserWithStats"/>.
        /// </summary>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> SelfAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var userWithStats = await _userWithStatsRepository.GetAsync(identityUser.Id).ConfigureAwait(false);
            return Ok(MapperUser.Map(userWithStats));
        }

        /// <summary>
        /// Updates the authenticated user.
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateInputModel input)
        {
            if (!ModelState.IsValid) { return BadRequest("Post body is invalid"); }

            var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var userEntity = await _userWithStatsRepository.GetAsync(identityUser.Id).ConfigureAwait(false);

            // TODO Question --> Also password changes in here? What kind of user changes do we want to be able to make?
            throw new NotImplementedException("Do we want to have our password changes in here as well?");
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
            throw new NotImplementedException();
            //var usersOutput =
            //    (await _followRequestRepository.GetOutgoingForUserAsync(userId))
            //    .Where(request => request.Status == FollowRequestStatus.Accepted)
            //    .Select(async request =>
            //    {
            //        var user = await _userService.GetAsync(request.RequesterId);
            //        UserOutputModel output = await CreateUserOutputModelAsync(user);
            //        return output;
            //    })
            //    .Select(t => t.Result);

            //return Ok(usersOutput);
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("self/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsSelfAsync()
        {
            var userId = (await _userManager.GetUserAsync(User)).Id;
            return await GetStatisticsAsync(userId);
        }

        /// <summary>
        /// Returns statistics of the specified user.
        /// </summary>
        [HttpGet("{userId}/statistics")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserStatisticsOutputModel))]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] Guid userId)
        {
            throw new NotImplementedException();
            //var userVlogs = await _vlogRepository.GetVlogsByUserAsync(userId);

            //int totalReactionsReceivedCount = 0;
            //int totalLikesReceivedCount = 0;

            ////TODO: Total views counter should be incremented in the for each loop for vlogs below
            //int totalViewsCount = int.MinValue;

            //// Add up statistical data for each of the users' vlogs.
            //foreach (var vlog in userVlogs)
            //{
            //    totalReactionsReceivedCount += await _reactionRepository.GetReactionCountForVlogAsync(vlog.Id);
            //    totalLikesReceivedCount += vlog.Likes.Count();

            //    //TODO: Add up received views for each vlog here: v
            //    //// totalViews += ?
            //}

            //return Ok(new UserStatisticsOutputModel
            //{
            //    TotalFollowers = await _followRequestRepository.GetFollowerCountAsync(userId),
            //    TotalFollowing = await _followRequestRepository.GetFollowingCountAsync(userId),
            //    TotalVlogs = await _vlogRepository.GetVlogCountForUserAsync(userId),
            //    TotalReactionsGiven = await _reactionRepository.GetGivenReactionCountForUserAsync(userId),
            //    TotalLikes = totalLikesReceivedCount,
            //    TotalReactionsReceived = totalReactionsReceivedCount,
            //    TotalViews = totalViewsCount
            //});
        }

        /// <summary>
        /// Get the followers of a single user.
        /// </summary>
        [HttpGet("{userId}/followers")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> ListFollowersAsync([FromRoute] Guid userId)
        {
            throw new NotImplementedException();
            //var followRelationships = await _followRequestRepository.GetIncomingForUserAsync(userId);

            //var usersOutput = followRelationships
            //    .Where(x => x.Status == FollowRequestStatus.Accepted)
            //    .Select(async x =>
            //    {
            //        var u = await _userService.GetAsync(x.RequesterId);
            //        UserOutputModel o = await CreateUserOutputModelAsync(u);
            //        return o;
            //    })
            //    .Select(t => t.Result);

            //return Ok(usersOutput);
        }

    }
}