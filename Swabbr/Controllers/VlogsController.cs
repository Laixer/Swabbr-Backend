﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.MockData;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
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
    [Authorize]
    [ApiController]
    [Route("api/v1/vlogs")]
    public class VlogsController : ControllerBase
    {
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public VlogsController(
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            UserManager<SwabbrIdentityUser> userManager
            )
        {
            _vlogRepository = vlogRepository;
            _vlogLikeRepository = vlogLikeRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Get a single vlog.
        /// </summary>
        [HttpGet("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> GetAsync([FromRoute]Guid vlogId)
        {
            //TODO Temporarily sending back mock data because there are no vlogs yet
            return Ok(MockRepository.RandomVlogOutput(vlogId));

            try
            {
                VlogOutputModel output = await _vlogRepository.GetByIdAsync(vlogId);
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Vlog could not be found.")
                    );
            }
        }

        /// <summary>
        /// Get a collection of featured vlogs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("featured")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> FeaturedAsync()
        {
            //TODO Not implemented, sending back fake data because there are no vlog records.
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 10));
        }

        // TODO Specify limit? pagination?
        /// <summary>
        /// Get vlogs from the specified user.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId)
        {
            var vlogs = await _vlogRepository.GetVlogsByUserAsync(userId);

            // Map the vlogs to their output model representations.
            IEnumerable<VlogOutputModel> output = vlogs
                .Select(v =>
                {
                    VlogOutputModel outputModel = v;
                    return outputModel;
                });

            return Ok(output);
        }

        /// <summary>
        /// Delete a vlog that is owned by the authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid vlogId)
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                var vlogEntity = await _vlogRepository.GetByIdAsync(vlogId);

                // Ensure the authenticated user is the owner of this vlog
                if (!vlogEntity.UserId.Equals(identityUser.UserId))
                {
                    return BadRequest(
                        this.Error(ErrorCodes.INSUFFICIENT_ACCESS_RIGHTS, "Access to this vlog is not allowed.")
                    );
                }

                // Delete the vlog
                await _vlogRepository.DeleteAsync(vlogEntity);

                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Vlog could not be found.")
                );
            }
        }

        // TODO What to return? Maybe an updated model of the vlog? Or the amount of likes for the vlog.
        /// <summary>
        /// Leave a like on a single vlog.
        /// </summary>
        [HttpPost("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> LikeAsync([FromRoute]Guid vlogId)
        {
            // Check if the vlog with the specified id exists
            //TODO: Check if the user has sufficient rights to access this vlog (shared users only for private vlogs)
            if (!(await _vlogRepository.ExistsAsync(vlogId)))
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Vlog could not be found.")
                );
            }

            var identityUser = await _userManager.GetUserAsync(User);

            // Create a new like record
            var entityToCreate = new VlogLike
            {
                VlogLikeId = Guid.NewGuid(),
                UserId = identityUser.UserId,
                VlogId = vlogId,
                TimeCreated = DateTime.Now,
            };

            await _vlogLikeRepository.CreateAsync(entityToCreate);

            return Ok();
        }

        /// <summary>
        /// Remove a like previously given to a single vlog.
        /// </summary>
        [HttpDelete("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UnlikeAsync([FromRoute]Guid vlogId)
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                // Retrieve and delete the entity
                var entityToDelete = await _vlogLikeRepository.GetSingleForUserAsync(vlogId, identityUser.UserId);
                await _vlogLikeRepository.DeleteAsync(entityToDelete);

                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Like could not be found.")
                );
            }
        }
    }
}