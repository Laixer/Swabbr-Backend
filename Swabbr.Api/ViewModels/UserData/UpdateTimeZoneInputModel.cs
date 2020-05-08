using Newtonsoft.Json;
using Swabbr.Api.Errors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// </summary>
        [RegularExpression(@"^UTC(\+|\-)\d{2}:\d{2}$", ErrorMessage = "Timezone must be in format UTC+xx:xx or UTC-xx:xx")]
        public string Timezone { get; set; }

    }

}
