using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob;
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
        private readonly IPlaybackService _livestreamPlaybackService;
        private readonly ILivestreamPoolService _livestreamPoolService;
        private readonly ILivestreamService _livestreamService;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
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
            IPlaybackService livestreamPlaybackService,
            ILivestreamService livestreamService,
            IDeviceRegistrationService deviceRegistrationService,
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
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _amsDebugService = debugService ?? throw new ArgumentNullException(nameof(debugService));
            _amsClient = amsClient ?? throw new ArgumentNullException(nameof(amsClient));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        [HttpPost("upload_reaction_video")]
        public async Task<IActionResult> AmsStorageTest(Guid reactionId, Uri sasUri)
        {
            if (!_hostingEnvironment.IsDevelopment()) { return Conflict($"Can only access {nameof(DebugController)} in development environment"); }

            try
            {
                var fileToUpload = @"C:\Users\thoma\Videos\Dora and Friends _ Doggie Day! _ Nick Jr. UK.mp4";
                var container = new CloudBlobContainer(sasUri);
                var blob = container.GetBlockBlobReference(Path.GetFileName(fileToUpload));
                await blob.UploadFromFileAsync(fileToUpload);
                return Ok();
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
                await _livestreamPoolService.CleanupLivestreamAsync(livestreamId);

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
