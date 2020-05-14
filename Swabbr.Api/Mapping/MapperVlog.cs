using Swabbr.Api.ViewModels.Vlog;
using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="Vlog"/> entities.
    /// </summary>
    internal static class MapperVlog
    {

        internal static VlogOutputModel Map(Vlog vlog)
        {
            if (vlog == null) { throw new ArgumentNullException(nameof(vlog)); }
            return new VlogOutputModel
            {
                DateStarted = vlog.StartDate,
                Id = vlog.Id,
                IsPrivate = vlog.IsPrivate,
                UserId = vlog.UserId,
                Views = vlog.Views
            };
        }

    }

}
