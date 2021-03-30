using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Extensions;
using Swabbr.Api.Helpers;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests related to users. 
    /// </summary>
    /// <remarks>
    ///     This does not process any authentication related requests.
    /// </remarks>
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly Core.AppContext _appContext;
        private readonly IUserService _userService;
        private readonly UserUpdateHelper _userUpdateHelper;
        private readonly IEntityStorageUriService _entityStorageUriService; // TODO Move to UserService? Do differently?
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserController(Core.AppContext appContext,
            IUserService userService,
            UserUpdateHelper userUpdateHelper,
            IEntityStorageUriService entityStorageUriService,
            IMapper mapper)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userUpdateHelper = userUpdateHelper ?? throw new ArgumentNullException(nameof(userUpdateHelper));
            _entityStorageUriService = entityStorageUriService ?? throw new ArgumentNullException(nameof(entityStorageUriService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/user/{id}
        /// <summary>
        ///     Get a user.
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid userId)
        {
            // Act.
            var user = await _userService.GetAsync(userId);

            // Map.
            var output = _mapper.Map<UserDto>(user);

            // Return.
            return Ok(output);
        }

        // GET: api/user/follow-requesting-users
        /// <summary>
        ///     Get wrappers around all users which have a follow 
        ///     request pending for our current user.
        /// </summary>
        [HttpGet("follow-requesting-users")]
        public async Task<IActionResult> GetFollowRequestingUsersAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var userWithRelationWrappers = await _userService.GetFollowRequestingUsersAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<UserWithRelationWrapperDto>>(userWithRelationWrappers);

            // Return.
            return Ok(output);
        }

        // GET: api/user/{id}/statistics
        /// <summary>
        ///     Get a user with its statistics.
        /// </summary>
        [HttpGet("{userId}/statistics")]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] Guid userId)
        {
            // Act.
            var user = await _userService.GetWithStatisticsAsync(userId);

            // Map.
            var output = _mapper.Map<UserWithStatsDto>(user);

            // Return.
            return Ok(output);
        }

        // GET: api/user/self/statistics
        /// <summary>
        ///     Get the current user with its statistics.
        /// </summary>
        /// <returns></returns>
        [HttpGet("self/statistics")]
        public async Task<IActionResult> GetStatisticsSelfAsync()
        {
            // Act.
            var user = await _userService.GetWithStatisticsAsync(_appContext.UserId);

            // Map.
            var output = _mapper.Map<UserWithStatsDto>(user);

            // Return.
            return Ok(output);
        }

        // GET: api/user/vlog-liking-users
        /// <summary>
        ///     Get wrappers around all vlog liking users for the current user.
        /// </summary>
        [HttpGet("vlog-liking-users")]
        public async Task<IActionResult> GetVlogLikingUsersAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogLikingUserWrappers = await _userService.GetVlogLikingUsersForUserAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogLikingUserWrapperDto>>(vlogLikingUserWrappers);

            // Return.
            return Ok(output);
        }

        // GET: api/user/{id}/following
        /// <summary>
        ///     List all users that a given user is following.
        /// </summary>
        [HttpGet("{userId}/following")]
        public async Task<IActionResult> ListFollowingAsync([FromRoute] Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var following = await _userService.GetFollowingAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<UserDto>>(following);

            // Return.
            return Ok(output);
        }

        // GET: api/user/{id}/followers
        /// <summary>
        ///     List all all users that are following a given user.
        /// </summary>
        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> ListFollowersAsync([FromRoute] Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var followers = await _userService.GetFollowersAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<UserDto>>(followers);

            // Return.
            return Ok(output);
        }

        // GET: api/user/search
        /// <summary>
        ///     Search for users based on their nickname.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] SearchDto input)
        {
            // Act.
            var users = await _userService.SearchAsync(input.Query, input.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<UserWithRelationWrapperDto>>(users);

            // Return.
            return Ok(output);
        }

        // GET: api/user/self
        /// <summary>
        ///     Get the current user.
        /// </summary>
        [HttpGet("self")]
        public async Task<IActionResult> SelfAsync()
        {
            // Act.
            var user = await _userService.GetAsync(_appContext.UserId);

            // Map.
            var output = _mapper.Map<UserCompleteDto>(user);
            // TODO Move to user service? Not controller responsibility. Right now this
            //      functionality only exists in the DTO layer, do something about that.
            output.ProfileImageUploadUri = await _entityStorageUriService.GetUserProfileImageUploadUriAsync(user.Id);

            // Return.
            return Ok(output);
        }

        // PUT: api/user
        /// <summary>
        ///     Update the current user.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto input)
        {
            // Act.
            await _userUpdateHelper.UpdateUserAsync(input);

            // Return.
            return NoContent();
        }
    }
}
