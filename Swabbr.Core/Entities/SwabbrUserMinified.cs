using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Contains only the id, the vlog request limit and the timezone
    /// of a <see cref="SwabbrUser"/>.
    /// </summary>
    public sealed class SwabbrUserMinified : EntityBase<Guid>
    {

        public int DailyVlogRequestLimit { get; set; }

        public TimeZoneInfo TimeZone { get; set; }

    }

}
