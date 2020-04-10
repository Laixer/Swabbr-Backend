using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Reaction
{
    public class ReactionWithDownloadOutputModel : ReactionOutputModel
    {

        public Uri VideoAccessUri { get; set; }

        public Uri ThumbnailAccessUri { get; set; }

    }
}
