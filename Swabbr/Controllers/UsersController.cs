using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling user related Api requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(
            IUserRepository repo,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager
            //UserManager<IdentityUser> userManager
        )
        {
            _repo = repo;
            _config = configuration;
            _signInManager = signInManager;
            //_userManager = userManager;
        }

        //TODO: Remove, temporary
        [HttpGet("tempdeletetables")]
        public async Task<IActionResult> TempDeleteTables()
        {
            await _repo.TempDeleteTables();
            return Ok();
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <param name="input">Input for a new user to register</param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            // Map input to model
            User user = input;

            // Generate new id for the new entity
            user.UserId = Guid.NewGuid();

            try
            {
                User createdUser = await _repo.AddAsync(user);

                // TODO?????????
                var xx = new IdentityUser
                {
                    UserName = input.Email,
                    Email = input.Email
                };
                //await _userManager.CreateAsync(xx, input.Password);

                return Created(Url.ToString(), createdUser);
            }
            catch (Exception e)
            {
                // TODO ??? What to do on error
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Authorizes a registered user.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginInputModel input)
        {
            //! TODO

            //var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

            //if (result.Succeeded)
            //{
            //    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == user.Email);
            //    var token = await GenerateAuthToken(user.Email, appUser);
            //    return Ok(token);
            //}

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [HttpDelete("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult Logout()
        {
            // TODO Deauthorize user, delete user access token
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information about a single user.
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Get([FromRoute]Guid userId)
        {
            try
            {
                User user = await _repo.GetByIdAsync(userId);
                UserOutputModel output = user;
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(userId);
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
        public async Task<IActionResult> Update([FromBody] Core.Entities.User user)
        {
            try
            {
                ////await _repo.UpdateAsync(user);
                ////return Ok(user);
            }
            catch
            {
                return BadRequest();
            }

            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the account of the authenticated user.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            Core.Entities.User user = null;
            ////await _repo.DeleteAsync(user.ToDocument());
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