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
    ///     Controller for handling requests vlog operations.
    /// </summary>
    [ApiController]
    [Route("vlog")]
    public sealed class VlogController : ControllerBase
    {
        private readonly IVlogService _vlogService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogController(IVlogService vlogService,
            IMapper mapper)
        { 
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // POST: api/vlog/add-views
        /// <summary>
        ///     Adds views to specified vlogs.
        /// </summary>
        [HttpPost("add-views")]
        public async Task<IActionResult> AddViewsAsync([FromBody] AddVlogViewsDto input)
        {
            // Act.
            var context = new AddVlogViewsContext
            {
                VlogViewPairs = input.VlogViewPairs,
            };
            await _vlogService.AddViews(context);

            // Return.
            return NoContent();
        }

        // DELETE: api/vlog/{id}
        /// <summary>
        ///     Deletes a vlog owned by the current user.
        /// </summary>
        /// <param name="vlogId"></param>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.DeleteAsync(vlogId);

            // Return.
            return NoContent();
        }

        // GET: api/vlog/{id}
        /// <summary>
        ///     Get a vlog.
        /// </summary>
        [HttpGet("{vlogId}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid vlogId)
        {
            // Act.
            var vlog = await _vlogService.GetAsync(vlogId);

            // Map.
            var output = _mapper.Map<VlogDto>(vlog);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog/{id}/with-summary
        /// <summary>
        ///     Get a vlog with its likes summarized.
        /// </summary>
        /// <param name="vlogId"></param>
        /// <returns></returns>
        [HttpGet("{vlogId}/with-summary")]
        public async Task<IActionResult> GetWithSummaryAsync([FromRoute] Guid vlogId)
        {
            // Act.
            var vlog = await _vlogService.GetAsync(vlogId);
            var vlogLikeSummary = await _vlogService.GetVlogLikeSummaryForVlogAsync(vlogId);

            // Map.
            var output = _mapper.Map<VlogWithSummaryDto>(vlog);
            output.TotalLikes = vlogLikeSummary.TotalLikes;
            output.Users = _mapper.Map<IEnumerable<UserDto>>(vlogLikeSummary.Users);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog/{id}/likes
        /// <summary>
        ///     Get likes for a vlog.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{vlogId}/likes")]
        public async Task<IActionResult> GetLikesForVlogAsync([FromRoute] Guid vlogId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var likes = await _vlogService.GetVlogLikesForVlogAsync(vlogId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogLikeDto>>(likes);

            // Return.
            return Ok(output);
        }
        
        // GET: api/vlog/recommended
        /// <summary>
        ///     Get recommended vlogs for the current user.
        /// </summary>
        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedVlogsAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetRecommendedForUserAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogDto>>(vlogs);

            // Return.
            return Ok(output);
        }
        
        // POST: api/vlog/{id}/like
        /// <summary>
        ///     Like a vlog.
        /// </summary>
        [HttpPost("{vlogId}/like")]
        public async Task<IActionResult> LikeAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.LikeAsync(vlogId);

            // Return.
            return NoContent();
        }

        // GET: api/vlog/for-user/{id}
        /// <summary>
        ///     List vlogs that are owned by a specified user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("for-user/{userId}")]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetVlogsByUserAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogDto>>(vlogs);

            // Return
            return Ok(output);
        }

        // POST: api/vlog
        /// <summary>
        ///     Post a new vlog as the current user.
        /// </summary>
        [HttpPost]
        public IActionResult PostVlog([FromServices] DispatchManager dispatchManager, [FromServices] Core.AppContext appContext, [FromBody] VlogDto input)
        {
            // Act.
            var postVlogContext = new PostVlogContext
            {
                IsPrivate = input.IsPrivate,
                UserId = appContext.UserId,
                VlogId = input.Id,
            };
            dispatchManager.Dispatch<PostVlogBackgroundTask>(postVlogContext);

            // Return.
            return NoContent();
        }

        // POST: api/vlog/{id}/unlike
        /// <summary>
        ///     Unlike a vlog.
        /// </summary>
        [HttpPost("{vlogId}/unlike")]
        public async Task<IActionResult> UnlikeAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.UnlikeAsync(vlogId);

            // Return.
            return NoContent();
        }

        // PUT: api/vlog/{id}
        /// <summary>
        ///     Update a vlog owned by the current user.
        /// </summary>
        [HttpPut("{vlogId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]Guid vlogId, [FromBody]VlogDto input)
        {
            // Map.
            var vlog = _mapper.Map<Vlog>(input);
            vlog.Id = vlogId;

            // Act.
            await _vlogService.UpdateAsync(vlog);

            // Return.
            return NoContent();
        }
    }
}
