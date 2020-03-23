﻿using Laixer.Utility.Extensions;
using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// <see cref="SwabbrUser"/> service.
    /// </summary>
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly SwabbrConfiguration config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserService(IUserRepository userRepository,
            IOptions<SwabbrConfiguration> options)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
        }

        public Task<bool> ExistsAsync(Guid userId)
        {
            return _userRepository.UserExistsAsync(userId);
        }

        public Task<SwabbrUser> GetAsync(Guid userId)
        {
            return _userRepository.GetAsync(userId);
        }

        public Task<SwabbrUser> GetByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }

        /// <summary>
        /// Gets the <see cref="UserSettings"/> for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        public Task<UserSettings> GetSettingsAsync(Guid userId)
        {
            userId.ThrowIfNullOrEmpty();
            return _userRepository.GetUserSettingsAsync(userId);
        }

        public Task<SwabbrUser> UpdateAsync(SwabbrUser user)
        {
            return _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Updates the <see cref="UserSettings"/> for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task UpdateSettingsAsync(UserSettings userSettings)
        {
            if (userSettings == null) { throw new ArgumentNullException(nameof(userSettings)); }
            userSettings.UserId.ThrowIfNullOrEmpty();

            if (userSettings.DailyVlogRequestLimit < 0) { throw new ArgumentOutOfRangeException(nameof(userSettings)); }
            if (userSettings.DailyVlogRequestLimit > config.DailyVlogRequestLimit) { throw new InvalidOperationException($"Daily vlog request limit can't be greater than {config.DailyVlogRequestLimit}"); }

            return _userRepository.UpdateUserSettingsAsync(userSettings);
        }

    }

}
