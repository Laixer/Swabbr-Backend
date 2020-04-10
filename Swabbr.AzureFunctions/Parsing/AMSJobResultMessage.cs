using System.Collections.Generic;

namespace Swabbr.AzureFunctions.Parsing
{

    /// <summary>
    /// Wrapper to bind json data from an AMS job result message.
    /// </summary>
    public sealed class AMSJobResultMessage
    {

        public IEnumerable<AMSJobResultOutputSub> Outputs { get; set; } = new List<AMSJobResultOutputSub>();

    }

    public sealed class AMSJobResultOutputSub
    {

        public string AssetName { get; set; }

    }

}
