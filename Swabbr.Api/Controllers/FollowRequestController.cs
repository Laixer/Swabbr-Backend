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
    [ApiController]
    [Route("followrequest")]
    public class FollowRequestController : ControllerBase
    {
        private readonly Core.AppContext _appContext;
        private readonly IFollowRequestService _followRequestService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public FollowRequestController(Core.AppContext appContext,
            IFollowRequestService followRequestService,
            IMapper mapper)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _followRequestService = followRequestService ?? throw new ArgumentNullException(nameof(followRequestService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptAsync(Guid requesterId)
        {
            // Act.
            await _followRequestService.AcceptAsync(requesterId);

            // Return.
            return NoContent();
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelAsync(Guid receiverId)
        {
            // Act.
            await _followRequestService.CancelAsync(receiverId);

            // Return.
            return NoContent();
        }

        [HttpPost("decline")]
        public async Task<IActionResult> DeclineAsync(Guid requesterId)
        {
            // Act.
            await _followRequestService.DeclineAsync(requesterId);

            // Return.
            return NoContent();
        }

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

        [HttpGet("outgoing/{receiverId}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid receiverId)
        {
            // Act.
            var followRequest = await _followRequestService.GetAsync(new FollowRequestId
            {
                ReceiverId = receiverId,
                RequesterId = _appContext.UserId
            });

            // Map.
            var output = _mapper.Map<FollowRequestDto>(followRequest);

            // Return.
            return Ok(output);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendAsync(Guid receiverId)
        {
            // Act.
            await _followRequestService.SendAsync(receiverId);

            // Return.
            return NoContent();
        }

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
