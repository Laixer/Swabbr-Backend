using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Swabbr.Api.Authentication;
using Swabbr.Api.Utility;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Debug functionality. This is only accessible from the local development
    /// machine. Sending requests to these endpoints will result in a conflict
    /// result.
    /// TODO Remove in final product in a non-beun way
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
        private readonly ILivestreamPoolService _livestreamPoolService;
        private readonly ILivestreamService _livestreamService;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly ITranscodingService _transcodingService;
        private readonly IReactionUploadService _reactionUploadService;
        private readonly IStorageService _storageService;
        private readonly IReactionRepository _reactionRepository;
        private readonly AMSDebugService _amsDebugService;
        private readonly IAMSClient _amsClient;
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DebugController(UserManager<SwabbrIdentityUser> userManager,
            IVlogTriggerService vlogTriggerService,
            INotificationService notificationService,
            ILivestreamPoolService livestreamPoolService,
            ILivestreamPlaybackService livestreamPlaybackService,
            ILivestreamService livestreamService,
            IDeviceRegistrationService deviceRegistrationService,
            ITranscodingService transcodingService,
            IReactionUploadService reactionUploadService,
            IStorageService storageService,
            IReactionRepository reactionRepository,
            AMSDebugService debugService,
            IAMSClient amsClient,
            IHostingEnvironment hostingEnvironment)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _livestreamPoolService = livestreamPoolService ?? throw new ArgumentNullException(nameof(livestreamPoolService));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
            _livestreamService = livestreamService ?? throw new ArgumentNullException(nameof(livestreamService));
            _deviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            _transcodingService = transcodingService ?? throw new ArgumentNullException(nameof(transcodingService));
            _reactionUploadService = reactionUploadService ?? throw new ArgumentNullException(nameof(reactionUploadService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _amsDebugService = debugService ?? throw new ArgumentNullException(nameof(debugService));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        [HttpPost("upload_test/{targetVlogId}")]
        [DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhysical([FromRoute] Guid targetVlogId)
        {
            if (!_hostingEnvironment.IsDevelopment()) { return Conflict($"Can only access {nameof(DebugController)} in development environment"); }

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
            if (!_hostingEnvironment.IsDevelopment()) { return Conflict($"Can only access {nameof(DebugController)} in development environment"); }

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

        [HttpPost("ams_debug")]
        public async Task<IActionResult> AmsDebug(Guid livestreamId)
        {
            try
            {
                await _amsDebugService.DoThingAsync().ConfigureAwait(false);

                //livestreamId = new Guid("f2192d61-ce08-4bb8-b955-60197ea2c507");
                //var liveEvent = await _amsClient.CreateLiveEventAsync(livestreamId).ConfigureAwait(false);
                //await _amsClient.StartLiveEventAsync(liveEvent.Name).ConfigureAwait(false);

                //liveEvent = await _amsDebugService.GetLiveEventAsync(liveEvent.Name).ConfigureAwait(false);
                //var accessUrl = liveEvent.Input.Endpoints.Where(x => x.Url.StartsWith("rtmps")).First().Url;
                //var accessToken = liveEvent.Input.AccessToken;

                //var liveEventManuallyCreated = await _amsDebugService.GetLiveEventAsync("live-event-manual").ConfigureAwait(false);
                //var accessUrlManual = liveEventManuallyCreated.Input.Endpoints.Where(x => x.Url.StartsWith("rtmps")).First().Url;
                //var accessTokenManual = liveEventManuallyCreated.Input.AccessToken;

                //await _amsClient.StopLiveEventAsync(liveEvent.Name).ConfigureAwait(false);
                //await _amsDebugService.DeleteLiveEventAsync(liveEvent.Name).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("trigger_vlog")]
        public async Task<IActionResult> VlogTrigger(Guid userId)
        {
            try
            {
                var hours = (int)755 / 60;
                var minutes = 755 - (60 * hours);
                var triggerMinute = new DateTimeOffset(2020, 05, 05, hours, minutes, 0, TimeSpan.Zero);
                await _vlogTriggerService.ProcessVlogTriggerForUserAsync(userId, triggerMinute);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("trigger_timeout")]
        public async Task<IActionResult> TriggerTimeout(Guid userId)
        {
            try
            {
                var hours = (int)755 / 60;
                var minutes = 755 - (60 * hours);
                var triggerMinute = new DateTimeOffset(2020, 05, 05, hours, minutes, 0, TimeSpan.Zero);
                await _vlogTriggerService.ProcessVlogTimeoutForUserAsync(userId, triggerMinute);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }
        }

    }

}
