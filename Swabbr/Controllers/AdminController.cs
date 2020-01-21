﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("v1/api/[controller]")]
    [ApiController]

    //TODO Marked as obsolete because the methods are not yet implemented
    [Obsolete]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public AdminController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        //TODO: Remove, temporary
        /// <summary>
        /// Used to remove tables to prevent unnecessary throughput billing
        /// </summary>
        /// <returns></returns>
        [HttpDelete("deletestoragetables")]
        public async Task<IActionResult> TempDeleteTables()
        {
            await userRepository.TempDeleteTables();
            return Ok();
        }

        [HttpPost("notifications/send")]
        public async Task<IActionResult> SendNotificationAsync()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ban a specific user account.
        /// </summary>
        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUserAsync()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a specific user account.
        /// </summary>
        [HttpDelete("users/{userId}/delete")]
        public async Task<IActionResult> DeleteUserAsync()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send out a warning to a specific user.
        /// </summary>
        [HttpPost("users/{userId}/warning")]
        public async Task<IActionResult> WarnUserAsync(string warningMessage)
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }
    }
}