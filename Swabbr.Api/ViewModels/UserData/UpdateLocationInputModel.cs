using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.UserData
{

    /// <summary>
    /// Viewmodel for updating the user location.
    /// </summary>
    public sealed class UpdateLocationInputModel
    {

        public double Longitude { get; set; }

        public double Latitude { get; set; }

    }

}
