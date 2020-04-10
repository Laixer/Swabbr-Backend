using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.UserData
{

    /// <summary>
    /// Viewmodel to update the user timezone.
    /// </summary>
    public sealed class UpdateTimeZoneInputModel
    {

        /// <summary>
        /// The specified timezone of the user
        /// TODO Regex this
        /// </summary>
        [JsonProperty("timezone")]
        //[RegularExpression(@"^UTC(\+|\-)\d{2}:\d{2}$", ErrorMessage = "Timezone must be in format UTC+xx:xx")]
        public string Timezone { get; set; }

    }

}
