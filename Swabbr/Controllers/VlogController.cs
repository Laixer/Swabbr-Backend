﻿using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.MockData;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to vlogs.
    /// </summary>
    [Obsolete("Vlog related requests are not supported yet.")]
    [ApiController]
    [Route("api/v1/vlogs")]
    public class VlogController : ControllerBase
    {
        private readonly IVlogRepository _repository;

        public VlogController(IVlogRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get a single vlog.
        /// </summary>
        [HttpGet("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> Get([FromRoute]Guid vlogId)
        {
            //TODO Not implemented
            return Ok(MockRepository.RandomVlogOutput());
        }

        /// <summary>
        /// Get a collection of featured vlogs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("featured")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> Featured()
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 10));
        }

        // TODO Specify limit?
        /// <summary>
        /// Get vlogs from the specified user.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> ListForUser([FromRoute]Guid userId)
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 5));
        }

        /// <summary>
        /// Delete a vlog that is owned by the authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete([FromRoute]Guid vlogId)
        {
            //TODO Not implemented
            return NoContent();
        }

        // TODO What to return? Maybe an updated model of the vlog? Or the amount of likes for the vlog.
        /// <summary>
        /// Leave a like on a single vlog.
        /// </summary>
        [HttpPost("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Like([FromRoute]Guid vlogId)
        {
            //TODO Not implemented, create a new vloglike for the vlog
            return Ok();
        }

        /// <summary>
        /// Remove a like previously given to a single vlog.
        /// </summary>
        [HttpDelete("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Unlike([FromRoute]Guid vlogId)
        {
            //TODO Not implemented
            return NoContent();
        }
    }
}