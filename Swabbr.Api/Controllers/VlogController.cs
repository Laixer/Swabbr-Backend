﻿using AutoMapper;
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
        /// <remarks>
        ///     By views, video views are meant.
        ///     This has nothing to do with ASP views.
        /// </remarks>
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
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid vlogId)
        {
            // Act.
            await _vlogService.DeleteAsync(vlogId);

            // Return.
            return NoContent();
        }

        // GET: api/vlog/generate-upload-uri
        /// <summary>
        ///     Get a signed uri for uploading a new vlog.
        /// </summary>
        [HttpGet("generate-upload-uri")]
        public async Task<IActionResult> Get()
        {
            // Act.
            var id = Guid.NewGuid();
            var result = await _vlogService.GenerateUploadUri(id);

            // Map.
            UploadWrapperDto output = _mapper.Map<UploadWrapperDto>(result);

            // Return.
            return Ok(output);
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
            VlogDto output = _mapper.Map<VlogDto>(vlog);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog/wrapper/{id}
        /// <summary>
        ///     Get a vlog wrapper.
        /// </summary>
        [HttpGet("wrapper/{vlogId}")]
        public async Task<IActionResult> GetWrapperAsync([FromRoute] Guid vlogId)
        {
            // Act.
            var vlog = await _vlogService.GetWrapperAsync(vlogId);

            // Map.
            VlogWrapperDto output = _mapper.Map<VlogWrapperDto>(vlog);

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
            IEnumerable<VlogDto> output = _mapper.Map<IEnumerable<VlogDto>>(vlogs);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog/wrappers-recommended
        /// <summary>
        ///     Get recommended vlog wrappers for the current user.
        /// </summary>
        [HttpGet("wrappers-recommended")]
        public async Task<IActionResult> GetRecommendedVlogWrappersAsync([FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetRecommendedWrappersForUserAsync(pagination.ToNavigation()).ToListAsync();

            // Map.
            IEnumerable<VlogWrapperDto> output = _mapper.Map<IEnumerable<VlogWrapperDto>>(vlogs);

            // Return.
            return Ok(output);
        }

        // GET: api/vlog/for-user/{id}
        /// <summary>
        ///     List vlogs that are owned by a specified user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("for-user/{userId}")]
        public async Task<IActionResult> ListForUserAsync([FromRoute] Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetVlogsByUserAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            IEnumerable<VlogDto> output = _mapper.Map<IEnumerable<VlogDto>>(vlogs);

            // Return
            return Ok(output);
        }

        // GET: api/vlog/wrappers-for-user/{id}
        /// <summary>
        ///     List vlog wrappers that are owned by a specified user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("wrappers-for-user/{userId}")]
        public async Task<IActionResult> ListWrappersForUserAsync([FromRoute] Guid userId, [FromQuery] PaginationDto pagination)
        {
            // Act.
            var vlogs = await _vlogService.GetVlogWrappersByUserAsync(userId, pagination.ToNavigation()).ToListAsync();

            // Map.
            IEnumerable<VlogWrapperDto> output = _mapper.Map<IEnumerable<VlogWrapperDto>>(vlogs);

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

        // PUT: api/vlog/{id}
        /// <summary>
        ///     Update a vlog owned by the current user.
        /// </summary>
        [HttpPut("{vlogId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid vlogId, [FromBody] VlogDto input)
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
