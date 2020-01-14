using Microsoft.Extensions.Hosting;
using Swabbr.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class VlogTriggerHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IVlogRepository _vlogRepository;
        private readonly IUserRepository _userRepository;

        public VlogTriggerHostedService(
            IVlogRepository vlogRepository, 
            IUserRepository userRepository)
        {
            _vlogRepository = vlogRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// An event that determines if a vlog request should be triggered and sent out to the user.
        /// </summary>
        /// <param name="state"></param>
        public async void OnTriggerEvent(object state)
        {
            var test = await _userRepository.GetByEmailAsync("beau@laixer.com");

            var vlogtest = await _vlogRepository.GetVlogCountForUserAsync(test.UserId);

            System.Diagnostics.Debug.WriteLine(nameof(VlogTriggerHostedService) + ": " + vlogtest);

            //TODO Determine which users to send a vlog request

            //TODO For each user that should be triggered:
            //TODO      Reserve a livestream for this user
            //TODO      Send out a notification to this user with the connection details
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(OnTriggerEvent, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, TimeSpan.Zero.Ticks);
            await Task.CompletedTask;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}