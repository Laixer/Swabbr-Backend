using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Wrapper class for a collection of <see cref="VlogOutputModel"/>s.
    /// </summary>
    public sealed class VlogCollectionOutputModel
    {

        public IEnumerable<VlogOutputModel> Vlogs { get; set; }

    }

}
