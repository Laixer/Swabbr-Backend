using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Api.ViewModels.Vlog
{
    /// <summary>
    ///     Wrapper class for a collection of <see cref="VlogWrapperOutputModel"/>s.
    /// </summary>
    public sealed class VlogCollectionOutputModel
    {
        /// <summary>
        ///     Total vlogs in this output model.
        /// </summary>
        public int VlogsCount => Vlogs.Count();

        /// <summary>
        ///     Collection of vlogs with their metadata.
        /// </summary>
        public IEnumerable<VlogWrapperOutputModel> Vlogs { get; set; }
    }
}
