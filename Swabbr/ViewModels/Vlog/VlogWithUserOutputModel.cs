using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels
{
    public class VlogWithUserOutputModel
    {
        public VlogOutputModel Vlog { get; set; }
        public UserOutputModel User { get; set; }
    }
}
