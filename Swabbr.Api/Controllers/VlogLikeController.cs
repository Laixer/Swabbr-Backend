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
    ///     Controller for handling requests vlog like operations.
    /// </summary>
    [ApiController]
    [Route("vlog-like")]
    public sealed class VlogLikeController : ControllerBase
    {
        private readonly IVlogLikeService _vlogLikeService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogLikeController(IVlogLikeService vlogLikeService,
            IMapper mapper)
        {
            _vlogLikeService = vlogLikeService ?? throw new ArgumentNullException(nameof(vlogLikeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/vlog-like/exits/{vlogId}/{userId}
        /// <summary>
        ///     Get a vlog.
        /// </summary>
        [HttpGet("exists/{vlogId}/{userId}")]
        public async Task<IActionResult> ExistsAsync([FromRoute] Guid vlogId, [FromRoute] Guid userId)
        {
            // Act.
            var exists = await _vlogLikeService.ExistsAsync(new VlogLikeId
            {
                VlogId = vlogId,
                UserId = userId
            });

            // Return.
            return Ok(exists);
        }

        // GET: api/vlog-like/{vlogId}/{userId}
        /// <summary>
        ///     Get a vlog like.
        /// </summary>
        [HttpGet("{vlogId}/{userId}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid vlogId, [FromRoute] Guid userId)
        {
            // Act.
            var vlogLike = await _vlogLikeService.GetAsync(new VlogLikeId
            {
                VlogId = vlogId,
                UserId = userId
            });

            // Map.
            var output = _mapper.Map<VlogLikeDto>(vlogLike);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog-like/summary/{id}
        /// <summary>
        ///     Get a summary of the likes for a vlog.
        /// </summary>
        /// <param name="vlogId"></param>
        /// <returns></returns>
        [HttpGet("summary/{vlogId}")]
        public async Task<IActionResult> GetVlogLikeSummaryAsync([FromRoute] Guid vlogId)
        {
            // Act.
            var vlogLikeSummary = await _vlogLikeService.GetVlogLikeSummaryForVlogAsync(vlogId);

            // Map.
            var output = _mapper.Map<VlogLikeSummaryDto>(vlogLikeSummary);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog-like/for-vlog/{vlogId}
        /// <summary>
        ///     Get likes for a vlog.
        /// </summary>
        [HttpGet("for-vlog/{vlogId}")]
        public async Task<IActionResult> GetLikesForVlogAsync([FromRoute] Guid vlogId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var likes = await _vlogLikeService.GetVlogLikesForVlogAsync(vlogId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogLikeDto>>(likes);

            // Return.
            return Ok(output);
        }

        // POST: api/vlog-like/like/{vlogId}
        /// <summary>
        ///     Like a vlog.
        /// </summary>
        [HttpPost("like/{vlogId}")]
        public async Task<IActionResult> LikeAsync([FromRoute] Guid vlogId)
        {
            // Act.
            await _vlogLikeService.LikeAsync(vlogId);

            // Return.
            return NoContent();
        }

        // POST: api/vlog-like/unlike/{vlogId}
        /// <summary>
        ///     Unlike a vlog.
        /// </summary>
        [HttpPost("unlike/{vlogId}")]
        public async Task<IActionResult> UnlikeAsync([FromRoute] Guid vlogId)
        {
            // Act.
            await _vlogLikeService.UnlikeAsync(vlogId);

            // Return.
            return NoContent();
        }
    }
}
