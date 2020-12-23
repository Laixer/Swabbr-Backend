using System;
using System.Collections.Generic;

namespace Swabbr.Core.Context
{
    /// <summary>
    ///     Context for adding views to a vlog.
    /// </summary>
    public class AddVlogViewsContext
    {
        /// <summary>
        ///     Id of the user that watched the vlogs.
        /// </summary>
        public Guid WatchingUserId { get; set; }

        /// <summary>
        ///     Dictionary containing the amount of views for each vlog.
        /// </summary>
        public Dictionary<Guid, uint> VlogViewPairs = new Dictionary<Guid, uint>();

        /// <summary>
        ///     Adds a given amount of views to a vlog id.
        /// </summary>
        /// <param name="vlogId">The vlog id</param>
        /// <param name="views">The amount of views.</param>
        public void AddForVlog(Guid vlogId, uint views)
        {
            if (vlogId == Guid.Empty)
            {
                throw new InvalidOperationException();
            }

            if (VlogViewPairs.ContainsKey(vlogId))
            {
                var currentViews = VlogViewPairs[vlogId];

                VlogViewPairs.Remove(vlogId);

                VlogViewPairs.Add(vlogId, currentViews + views);
            }
            else
            {
                VlogViewPairs.Add(vlogId, views);
            }
        }
    }
}
