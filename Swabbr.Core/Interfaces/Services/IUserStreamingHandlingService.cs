﻿using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for managing user stream requests.
    /// TODO Update docs
    /// </summary>
    public interface IUserStreamingHandlingService
    {

        /// <summary>
        /// Creates a vlog for the user
        /// Notifies all followers
        /// Returns the vlog
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="livestreamId"></param>
        /// <returns></returns>
        Task<Vlog> OnUserStartStreaming(Guid userId, Guid livestreamId);

        /// <summary>
        /// Does db markings
        /// Starts the vlogdownload service in some async way
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="livestreamId"></param>
        /// <returns></returns>
        Task OnUserStopStreaming(Guid userId, Guid livestreamId);

    }

}