using AutoMapper;
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
    // TODO All docs for controllers
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

        [HttpPost("{vlogId}/addview")]
        public async Task<IActionResult> AddViewAsync([FromRoute] Guid vlogId)
        {
            // Act.
            await _vlogService.AddView(vlogId);

            // Return.
            return NoContent();
        }

        [HttpDelete("{vlogId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.DeleteAsync(vlogId);

            // Return.
            return NoContent();
        }

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

        [HttpGet("{vlogId}/withsummary")]
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

        [HttpGet("{vlogId}/likes")]
        public async Task<IActionResult> GetLikesForVlogAsync([FromRoute] Guid vlogId, [FromQuery] PaginationDto pagination)
        {
            // TODO ToListAsync eruit en mapper extension voor bouwen?
            // Act.
            var likes = await _vlogService.GetVlogLikesForVlogAsync(vlogId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogLikeDto>>(likes);

            // Return.
            return Ok(output);
        }
        
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
        
        [HttpPost("{vlogId}/like")]
        public async Task<IActionResult> LikeAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.LikeAsync(vlogId);

            // Return.
            return NoContent();
        }

        [HttpGet("foruser/{userId}")]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetVlogsByUserAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            var output = _mapper.Map<IEnumerable<VlogDto>>(vlogs);

            // Return
            return Ok(output);
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostAsync([FromBody] VlogDto input)
        {
            // Act.
            await _vlogService.PostVlogAsync(input.Id, input.IsPrivate);

            // Return.
            return NoContent();
        }

        [HttpPost("{vlogId}/unlike")]
        public async Task<IActionResult> UnlikeAsync([FromRoute]Guid vlogId)
        {
            // Act.
            await _vlogService.UnlikeAsync(vlogId);

            // Return.
            return NoContent();
        }

        [HttpPost("{vlogId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]Guid vlogId, [FromBody]VlogDto input)
        {
            // Act.
            var vlog = await _vlogService.GetAsync(vlogId);
            vlog.IsPrivate = input.IsPrivate;

            await _vlogService.UpdateAsync(vlog);

            // Return.
            return NoContent();
        }
    }
}
