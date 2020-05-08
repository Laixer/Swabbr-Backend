using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for managing hash distributions for timed triggers.
    /// </summary>
    public interface IHashDistributionService
    {

        IEnumerable<SwabbrUserMinified> GetForMinute(IEnumerable<SwabbrUserMinified> users, DateTimeOffset time, TimeSpan? offset = null);

    }

}
