using Swabbr.WowzaStreamingCloud.Enums;
using System;

namespace Swabbr.WowzaStreamingCloud.Parsing
{

    /// <summary>
    /// Contains functionality to parse Wowza enums.
    /// </summary>
    internal static class WscEnumParser
    {

        /// <summary>
        /// Parses a <see cref="WscYesNo"/> to its corresponding <see cref="bool"/>.
        /// </summary>
        /// <param name="value"><see cref="WscYesNo"/></param>
        /// <returns><see cref="bool"/></returns>
        internal static bool Parse(WscYesNo value)
        {
            switch (value)
            {
                case WscYesNo.Yes:
                    return true;
                case WscYesNo.No:
                    return false;
            }

            throw new InvalidOperationException(nameof(value));
        }

    }

}
