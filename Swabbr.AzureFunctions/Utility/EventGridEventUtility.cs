using Microsoft.Azure.EventGrid.Models;
using System;

namespace Swabbr.AzureFunctions.Utility
{
    /// <summary>
    /// Contains utility functionality for handling Azure Event Grid messages.
    /// </summary>
    internal static class EventGridEventUtility
    {
        /// <summary>
        /// Extracts the name of a live event from an <paramref name="eventGridEvent"/>.
        /// </summary>
        /// <param name="eventGridEvent"><see cref="EventGridEvent"/></param>
        /// <returns>AMS Live Event name</returns>
        internal static string ExtractLiveEventExternalId(EventGridEvent eventGridEvent)
        {
            if (eventGridEvent == null) { throw new ArgumentNullException(nameof(eventGridEvent)); }
            if (eventGridEvent.Subject == null) { throw new ArgumentNullException(nameof(eventGridEvent.Subject)); }
            var split = eventGridEvent.Subject.Split('/');
            if (split.Length != 2) { throw new FormatException("Could not extract AMS live event external id from event grid event"); }
            return split[1];
        }
    }
}
