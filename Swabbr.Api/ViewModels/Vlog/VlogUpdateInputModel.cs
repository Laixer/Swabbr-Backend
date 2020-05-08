using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Input model for updating a <see cref="Vlog"/>.
    /// </summary>
    public class VlogUpdateInputModel
    {

        /// <summary>
        /// A collection of user id's to share the vlog with.
        /// </summary>
        public List<Guid> SharedUsers { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

    }

}
