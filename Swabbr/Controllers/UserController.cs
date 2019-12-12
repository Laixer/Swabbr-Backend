using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Infrastructure.Data.Entities;
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
        private readonly ITokenService _tokenService;
        private readonly UserManager<IdentityUserTableEntity> _userManager;
        private readonly SignInManager<IdentityUserTableEntity> _signInManager;

        public UserController(
            IUserService service,
            ITokenService tokenService,
            UserManager<IdentityUserTableEntity> userManager,
            SignInManager<IdentityUserTableEntity> signInManager)
        {
            _service = service;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //TODO: Remove, temporary
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("deletetables")]
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
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(UserOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            // Convert input model to an identity user model
            User user = input;

            //TODO Do this here?
            // Generate new id
            user.UserId = Guid.NewGuid();

            var u = new IdentityUserTableEntity { UserId = Guid.NewGuid(), Email = input.Email };

            try
            {
                var x = await _userManager.CreateAsync(u, input.Password);

                if (x.Succeeded)
                {
                    await _signInManager.SignInAsync(u, true);
                    return Ok(_tokenService.GenerateToken(u));

                    UserOutputModel userOutput = user;
                    return Created(Url.ToString(), userOutput);
                }
                else
                {
                    return BadRequest();
                }
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
            IdentityUserTableEntity user = new IdentityUserTableEntity
            {
                Email = input.Email,
                UserId = Guid.NewGuid()
            };

            // Attempt a sign in
            var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, true, false);

            // logged in
            if (result.Succeeded)
            {
                var token = _tokenService.GenerateToken(user);
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
        [HttpGet("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel[]))]
        public async Task<IActionResult> Search(
            [FromQuery]string q)
        {
            var mockData = new UserOutputModel[]
            {
                UserOutputModel.NewRandomMock(),
                UserOutputModel.NewRandomMock(),
                UserOutputModel.NewRandomMock()
            };

            return Ok(mockData);
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Self()
        {
            var identity = User.Identity;
            var claims = User.Claims;

            string testb = "";

            testb += (identity.IsAuthenticated ? "YES./" : "NO./");
            testb += ($"TYPE:{identity.AuthenticationType}/");
            testb += ($"NAME:{identity.Name}/");

            foreach (var c in claims)
            {
                testb += $"{c.Value}/";
            }

            return Ok(testb);

            var userId = _userManager.GetUserId(HttpContext.User);

            return Ok(userId);

            return Ok(UserOutputModel.NewRandomMock());
            //! TODO
            //Get authenticated user id, get and return associated user vm
        }

        /// <summary>
        /// Update the authenticated user.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Update([FromBody] Core.Entities.User user)
        {
            return Ok(user);

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
            return NoContent();
            ////await _repo.DeleteAsync(user.ToDocument());
            //! TODO
            throw new NotImplementedException();
        }
    }
}