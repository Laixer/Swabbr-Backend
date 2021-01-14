using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Extensions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for follow request related operations.
    /// </summary>
    /// <remarks>
    ///     Often the requesterId or receiverId are explicitly
    ///     required as query parameters. This is in order to
    ///     prevent confusion with requesting and receiving ids.
    /// </remarks>
    [ApiController]
    [Route("followrequest")]
    public class FollowRequestController : ControllerBase
    {
        private readonly IFollowRequestService _followRequestService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public FollowRequestController(IFollowRequestService followRequestService,
            IMapper mapper)
        {
            _followRequestService = followRequestService ?? throw new ArgumentNullException(nameof(followRequestService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // PUT: api/followrequest/accept
        /// <summary>
        ///     Accept a pending incoming follow request.
        /// </summary>
        [HttpPut("accept")]
        public async Task<IActionResult> AcceptAsync(Guid requesterId)
        {
            // Act.
            await _followRequestService.AcceptAsync(requesterId);

            // Return.
            return NoContent();
        }

        // PUT: api/followrequest/cancel 
        /// <summary>
        ///     Cancel a pending outgoing follow request.
        /// </summary>
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelAsync(Guid receiverId)
        {
            // Act.
            await _followRequestService.CancelAsync(receiverId);

            // Return.
            return NoContent();
        }

        // PUT: api/followrequest/decline
        /// <summary>
        ///     Decline a pending incoming follow request.
        /// </summary>
        [HttpPut("decline")]
        public async Task<IActionResult> DeclineAsync(Guid requesterId)
        {
            // Act.
            await _followRequestService.DeclineAsync(requesterId);

            // Return.
            return NoContent();
        }

        // GET: api/followrequest/incoming
        /// <summary>
        ///     Get all incoming pending follow requests for the current user.
        /// </summary>
        [HttpGet("incoming")]
        public async Task<IActionResult> IncomingAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var outgoing = await _followRequestService.GetPendingIncomingForUserAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<FollowRequestDto>>(outgoing);

            // Return.
            return Ok(output);
        }

        // GET: api/followrequest/outgoing
        /// <summary>
        ///     Get all outgoing pending follow requests for the current user.
        /// </summary>
        [HttpGet("outgoing")]
        public async Task<IActionResult> OutgoingAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var outgoing = await _followRequestService.GetPendingOutgoingForUserAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<FollowRequestDto>>(outgoing);

            // Return.
            return Ok(output);
        }
        
        // FUTURE Do we need a get?

        // POST: api/followrequest
        /// <summary>
        ///     Send a new follow request from the current user to a given user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendAsync(Guid receiverId)
        {
            // Act.
            await _followRequestService.SendAsync(receiverId);

            // Return.
            return NoContent();
        }

        // POST: api/followrequest/unfollow
        /// <summary>
        ///     Unfollow a given user as the current user.
        /// </summary>
        [HttpPost("unfollow")]
        public async Task<IActionResult> UnfollowAsync(Guid receiverId)
        {
            // Act.
            await _followRequestService.UnfollowAsync(receiverId);

            // Return.
            return NoContent();
        }
    }
}
