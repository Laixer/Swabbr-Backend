﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Swabbr.ViewModels;
using Swabbr.Core.Models;
using Swabbr.Core.Documents;

namespace Swabbr.Api.Controllers
{
    //TODO Determine attributes
    /// <summary>
    /// Controller for handling user related Api requests.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public UsersController(IUserRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _config = configuration;
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <param name="user">New user information</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            try
            {
                UserDocument user = null;

                var x = await _repo.AddAsync(user);
                return Created(Url.ToString(), x);
            }
            catch
            {
                // TODO ??? What to do
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Authorizes a registered user.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            // TODO Deauthorize user, delete user access token
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information about a single user.
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute]string userId)
        {
            try
            {
                var user = await _repo.GetByIdAsync(userId);
                return Ok(user);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="q">Search query.</param>
        /// <param name="offset">To be used for pagination.</param>
        /// <param name="limit">Maximum amount of results to fetch.</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery]string q,
            [FromQuery]uint offset = 0,
            [FromQuery]uint limit = 100)
        {
            var results = await _repo.SearchAsync(q, offset, limit);

            return Ok(results);

            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        public async Task<IActionResult> Self()
        {
            //! TODO

            //Get authenticated user id, get and return associated user vm
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the authenticated user.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            try
            {
                await _repo.UpdateAsync(user.ToDocument());
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }

            //! TODO
            throw new NotImplementedException();
        }

        //TODO Should these contain a reference to the requirements?
        /// <summary>
        /// Delete the account of the authenticated user.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            User user = null;
            await _repo.DeleteAsync(user.ToDocument());
            //! TODO
            throw new NotImplementedException();
        }

        private async Task<object> GenerateAuthToken(string emailAddress, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, emailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _config["JwtIssuer"],
                _config["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}