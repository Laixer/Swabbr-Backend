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

#pragma warning disable CA1062 // Validate arguments of public methods
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
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserController(Core.AppContext appContext,
            IUserService userService,
            UserUpdateHelper userUpdateHelper,
            IMapper mapper)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userUpdateHelper = userUpdateHelper ?? throw new ArgumentNullException(nameof(userUpdateHelper));
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
            var output = _mapper.Map<IEnumerable<UserDto>>(users);

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
#pragma warning restore CA1062 // Validate arguments of public methods
