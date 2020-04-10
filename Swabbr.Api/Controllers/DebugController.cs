using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Swabbr.Api.Authentication;
using Swabbr.Api.Utility;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Debug functionality.
    /// TODO Indicate this debug functionality for the final product!
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("debug")]
    public class DebugController : ApiControllerBase
    {

        private readonly IVlogTriggerService _vlogTriggerService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ILivestreamPlaybackService _livestreamPlaybackService;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly ITranscodingService _transcodingService;
        private readonly IReactionUploadService _reactionUploadService;
        private readonly IStorageService _storageService;
        private readonly IReactionRepository _reactionRepository;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DebugController(IVlogTriggerService vlogTriggerService,
            UserManager<SwabbrIdentityUser> userManager,
            INotificationService notificationService,
            ILivestreamPlaybackService livestreamPlaybackService,
            IDeviceRegistrationService deviceRegistrationService,
            ITranscodingService transcodingService,
            IReactionUploadService reactionUploadService,
            IStorageService storageService,
            IReactionRepository reactionRepository)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
            _deviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            _transcodingService = transcodingService ?? throw new ArgumentNullException(nameof(transcodingService));
            _reactionUploadService = reactionUploadService ?? throw new ArgumentNullException(nameof(reactionUploadService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
        }

        [HttpGet("fastly_token_test")]
        public async Task<IActionResult> FastlyTokenTest(Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var token = await _livestreamPlaybackService.GetTokenAsync(livestreamId, user.Id).ConfigureAwait(false);
                return Ok(token);
            }
            catch (Exception e)
            {
                return Conflict(e.Message); // TODO UNSAFE, change
            }
        }

        [HttpPost("register_device_for_user")]
        public async Task<IActionResult> RegisterDeviceForUser(Guid userId, string deviceHandle)
        {
            userId.ThrowIfNullOrEmpty();
            deviceHandle.ThrowIfNullOrEmpty();

            await _deviceRegistrationService.RegisterOnlyThisDeviceAsync(userId, PushNotificationPlatform.FCM, deviceHandle).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("upload_test/{targetVlogId}")]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhysical([FromRoute] Guid targetVlogId)
        {
            try
            {
                if (targetVlogId.IsNullOrEmpty()) { return BadRequest("target vlog id empty"); }
                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType)) { return BadRequest("No multipart"); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), lengthLimit: 70);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                // This creates a new reaction in the DB and gets an upload stream
                var reactionId = Guid.Empty; // TODO Kind of a beunfix
                using (var streamWrapper = await _reactionUploadService.GetReactionUploadStreamAsync(user.Id, targetVlogId).ConfigureAwait(false))
                {
                    reactionId = streamWrapper.EntityId;
                    while (section != null)
                    {
                        if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                        {
                            // This check assumes that there's a file
                            // present without form data. If form data
                            // is present, this method immediately fails
                            // and returns the model error.
                            if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                            {
                                ModelState.AddModelError("File", $"The request couldn't be processed (Error 2).");
                                return BadRequest(ModelState);
                            }
                            else
                            {
                                var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                                    section, contentDisposition, ModelState,
                                    permittedExtensions: new string[] { ".mp4" },
                                    sizeLimit: 9999999999); // TODO Size limit

                                // The filehelpers.processstreamedfile adds errors to the model state if something's off
                                if (!ModelState.IsValid) { return BadRequest(ModelState); }

                                await streamWrapper.Stream.WriteAsync(streamedFileContent);
                            }
                        }

                        section = await reader.ReadNextSectionAsync();
                    }
                }

                // This triggers the upload to be finished
                await _reactionUploadService.OnFinishedUploadingReactionAsync(user.Id, reactionId).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("amsstoragetest")]
        public async Task<IActionResult> AmsStorageTest(Guid reactionId)
        {
            try
            {
                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);

                reactionId.ThrowIfNullOrEmpty();
                var length = await _transcodingService.ExtractVideoLengthInSecondsAsync(reactionId).ConfigureAwait(false);
                return Ok(length);
                // await _reactionUploadService.OnFinishedTranscodingReactionAsync(reactionId).ConfigureAwait(false);
                // return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

    }
}




