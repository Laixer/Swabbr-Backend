using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Wrapper class for a collection of <see cref="VlogOutputModel"/>s.
    /// </summary>
    public sealed class VlogCollectionOutputModel
    {

        public int VlogsCount => Vlogs.Count();

        public IEnumerable<VlogOutputModel> Vlogs { get; set; }

    }

}
