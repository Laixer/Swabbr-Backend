using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests related to users. 
    /// </summary>
    /// <remarks>
    ///     This does not process any authentication related requests.
    /// </remarks>
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserController(IUserService userService,
            IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid userId)
        {
            // Act.
            var user = await _userService.GetAsync(userId);

            // Map.
            var output = _mapper.Map<UserDto>(user);

            // Return.
            return Ok(user);
        }

        [HttpGet("{userId}/statistics")]
        public async Task<IActionResult> GetStatisticsAsync([FromRoute] Guid userId)
        {
            // Act.
            var user = await _userService.GetWithStatisticsAsync(userId);

            // Map.
            var output = _mapper.Map<UserWithStatsDto>(user);

            // Return.
            return Ok(user);
        }

        [HttpGet("self/statistics")]
        public async Task<IActionResult> GetStatisticsSelfAsync([FromServices] UserManager<SwabbrIdentityUser> userManager)
        {
            // Act.
            var user = await _userService.GetWithStatisticsAsync(Guid.Parse(userManager.GetUserId(User)));

            // Map.
            var output = _mapper.Map<UserWithStatsDto>(user);

            // Return.
            return Ok(user);
        }

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

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string query, [FromQuery] PaginationDto pagination)
        {
            // Act.
            // TODO Ugly
            if (query.Length <= 3)
            {
                return Conflict("Minimum search length is 3");
            }

            var users = await _userService.SearchAsync(query, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<UserDto>>(users);

            // Return.
            return Ok(users);
        }

        // TODO Ugly
        [HttpGet("self")]
        public async Task<IActionResult> SelfAsync([FromServices] UserManager<SwabbrIdentityUser> userManager)
        {
            // Act.
            var user = await _userService.GetAsync(Guid.Parse(userManager.GetUserId(User)));

            // Map.
            var output = _mapper.Map<UserDto>(user);

            // Return.
            return Ok(output);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto input, [FromServices] UserManager<SwabbrIdentityUser> userManager)
        {
            // Act.
            var currentUser = await _userService.GetAsync(Guid.Parse(userManager.GetUserId(User)));

            // TODO Double functionality with IUserService impmlementation
            currentUser.BirthDate = input.BirthDate ?? currentUser.BirthDate;
            currentUser.Country = input.Country ?? currentUser.Country;
            currentUser.DailyVlogRequestLimit = input.DailyVlogRequestLimit ?? currentUser.DailyVlogRequestLimit;
            currentUser.FirstName = input.FirstName ?? currentUser.FirstName;
            currentUser.FollowMode = input.FollowMode ?? currentUser.FollowMode;
            currentUser.Gender = input.Gender ?? currentUser.Gender;
            currentUser.IsPrivate = input.IsPrivate ?? currentUser.IsPrivate;
            currentUser.LastName = input.LastName ?? currentUser.LastName;
            currentUser.Latitude = input.Latitude ?? currentUser.Latitude;
            currentUser.Longitude = input.Longitude ?? currentUser.Longitude;
            currentUser.Nickname = input.Nickname ?? currentUser.Nickname;
            currentUser.ProfileImageBase64Encoded = input.ProfileImageBase64Encoded ?? currentUser.ProfileImageBase64Encoded;
            currentUser.Timezone = input.Timezone ?? currentUser.Timezone;

            await _userService.UpdateAsync(currentUser);

            // Return.
            return NoContent();
        }
    }
}
