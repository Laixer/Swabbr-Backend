﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Swabbr.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpPost("notifications/send")]
        public async Task<IActionResult> SendNotification()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ban a specific user account.
        /// </summary>
        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUser()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a specific user account.
        /// </summary>
        [HttpDelete("users/{userId}/delete")]
        public async Task<IActionResult> DeleteUser()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send out a warning to a specific user.
        /// </summary>
        [HttpPost("users/{userId}/warning")]
        public async Task<IActionResult> WarnUser(string warningMessage)
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }
    }
}