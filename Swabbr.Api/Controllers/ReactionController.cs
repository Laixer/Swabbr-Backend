using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Extensions;
using Swabbr.Core.BackgroundTasks;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Context;
using Swabbr.Core.Entities;
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
    [ApiController]
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

        // DELETE: api/reaction/{id}
        /// <summary>
        ///     Delete a reaction owned by the current user.
        /// </summary>
        [HttpDelete("{reactionId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid reactionId)
        {
            // Act.
            await _reactionService.DeleteReactionAsync(reactionId);

            // Return.
            return NoContent();
        }

        // GET: api/reaction/upload-uri
        /// <summary>
        ///     Get a signed uri for uploading a new reaction.
        /// </summary>
        [HttpGet("generate-upload-uri")]
        public async Task<IActionResult> Get()
        {
            // Act.
            var id = Guid.NewGuid();
            var result = await _reactionService.GenerateUploadDetails(id);

            // Map.
            var output = _mapper.Map<UploadWrapperDto>(result);

            // Return.
            return Ok(output);
        }

        // GET: api/reaction/{id}
        /// <summary>
        ///     Get a reaction.
        /// </summary>
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

        // GET: api/reaction/for-vlog/{id}
        /// <summary>
        ///     Get all reactions for a given vlog.
        /// </summary>
        [HttpGet("for-vlog/{vlogId}")]
        public async Task<IActionResult> GetReactionsForVlog([FromRoute] Guid vlogId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var reactions = await _reactionService.GetReactionsForVlogAsync(vlogId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<ReactionDto>>(reactions);

            // Return.
            return Ok(output);
        }

        // GET: api/reaction/for-vlog/{id}/count
        /// <summary>
        ///     Get the reaction count for a given vlog.
        /// </summary>
        [HttpGet("for-vlog/{vlogId}/count")]
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

        // POST: api/reaction
        /// <summary>
        ///     Post a new reaction as the current user.
        /// </summary>
        [HttpPost]
        public IActionResult PostReaction([FromServices] DispatchManager dispatchManager, [FromServices] Core.AppContext appContext, [FromBody] ReactionDto input)
        {
            // Act.
            var postReactionContext = new PostReactionContext
            {
                IsPrivate = input.IsPrivate,
                ReactionId = input.Id,
                TargetVlogId = input.TargetVlogId,
                UserId = appContext.UserId,
            };
            dispatchManager.Dispatch<PostReactionBackgroundTask>(postReactionContext);

            // Return.
            return NoContent();
        }

        // PUT: api/reaction/{id}
        /// <summary>
        ///     Update a reaction owned by the current user.
        /// </summary>
        [HttpPut("{reactionId}")]
        public async Task<IActionResult> Update([FromRoute] Guid reactionId, [FromBody] ReactionDto input)
        {
            // Map.
            var reaction = _mapper.Map<Reaction>(input);
            reaction.Id = reactionId;

            // Act.
            await _reactionService.UpdateReactionAsync(reaction);

            // Return.
            return NoContent();
        }
    }
}
