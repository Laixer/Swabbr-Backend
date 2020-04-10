using Laixer.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Swabbr.AzureMediaServices.Utility
{
   
    /// <summary>
    /// Extracts id's from asset names.
    /// </summary>
    public static class AMSAssetNameIdExtractor
    {

        private static string format => @"^reaction-output-{0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";

        public static Guid GetId(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();
            if (!Regex.IsMatch(assetName, format)) { throw new FormatException("Invalid asset name format for id extraction"); }
            var idSubString = assetName.Substring(16);
            return new Guid(idSubString);
        }

    }

}
