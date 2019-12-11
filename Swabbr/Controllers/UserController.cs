using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling user related Api requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        //private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly IUserStore<AzureTableIdentityUser> _userStore;
        //private readonly SignInManager<AzureTableIdentityUser> _signInManager;

        public UserController(IUserService service)
        {
            _service = service;
        }

        //TODO: Remove, temporary
        [HttpGet("IGNORE_temporaryMethodUsedToDeleteTablesForTestingPurposesOnly")]
        public async Task<IActionResult> TempDeleteTables()
        {
            await _service.TempDeleteTables();
            return Ok();
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <param name="input">Input for a new user to register</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            // Convert input model to an identity user model
            User user = input;

            //TODO Do this here?
            // Generate new id
            user.UserId = Guid.NewGuid();

            try
            {
                // TODO: What to do with  the cancellation token?
                //var result = await _userStore.CreateAsync(identityUser, new System.Threading.CancellationToken());

                return Created(Url.ToString(), null);
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
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserAuthenticateModel input)
        {
            //! TODO

            //var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);

            //if (result.Succeeded)
            //{
            //    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == user.Email);
            //    var token = await GenerateAuthToken(user.Email, appUser);
            //    return Ok(token);
            //}

            var token = await _service.Authenticate(input.Email, input.Password);

            if (token == null)
            {
                // Not authorized
                return new UnauthorizedResult();
            }
            else
            {
                // Authorized, return the access token
                return Ok(token);
            }

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
                User user = await _service.GetByIdAsync(userId);
                UserOutputModel output = user;
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(userId);
            }
        }

        // TODO remove unused parameters
        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="q">Search query.</param>
        /// <param name="offset">To be used for pagination.</param>
        /// <param name="limit">Maximum amount of results to fetch.</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery]string q)
        {
            var results = await _service.SearchAsync(q);

            return Ok(results);
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete()
        {
            ////await _repo.DeleteAsync(user.ToDocument());
            //! TODO
            throw new NotImplementedException();
        }
    }
}