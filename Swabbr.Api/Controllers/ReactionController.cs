using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Extensions;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests related to reactions.
    /// </summary>
    [Authorize]
    [Route("reaction")]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ReactionController(IReactionService reactionService,
            IMapper mapper)
        {
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpDelete("{reactionId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid reactionId)
        {
            // Act.
            await _reactionService.DeleteReactionAsync(reactionId);

            // Return.
            return NoContent();
        }

        [HttpGet("{reactionId}")]
        public async Task<IActionResult> Get([FromRoute] Guid reactionId)
        {
            // Act.
            var result = await _reactionService.GetAsync(reactionId);

            // Map.
            var output = _mapper.Map<ReactionDto>(result);

            // Return.
            return Ok(output);
        }

        [HttpGet("forvlog/{vlogId}")]
        public async Task<IActionResult> GetReactionsForVlog([FromRoute] Guid vlogId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var reactions = await _reactionService.GetReactionsForVlogAsync(vlogId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<ReactionDto>>(reactions);

            // Return.
            return Ok(output);
        }

        [HttpGet("forvlog/{vlogId}/count")]
        public async Task<IActionResult> GetReactionCountForVlog([FromRoute] Guid vlogId)
        {
            // Act.
            var count = await _reactionService.GetReactionCountForVlogAsync(vlogId);

            // Map.
            var output = new DatasetStatsDto
            {
                Count = count
            };

            // Return.
            return Ok(output);
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostReactionAsync([FromBody] ReactionDto input)
        {
            // Act.
            await _reactionService.PostReactionAsync(input.TargetVlogId, input.Id);

            // Return.
            return NoContent();
        }

        [HttpPost("{reactionId}/update")]
        public async Task<IActionResult> Update([FromRoute] Guid reactionId, [FromBody] ReactionDto input)
        {
            // Act.
            var reaction = await _reactionService.GetAsync(reactionId);
            reaction.IsPrivate = input.IsPrivate;

            await _reactionService.UpdateReactionAsync(reaction);

            // Return.
            return NoContent();
        }
    }
}
