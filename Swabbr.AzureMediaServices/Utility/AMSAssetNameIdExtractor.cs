using Laixer.Utility.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Swabbr.AzureMediaServices.Utility
{

    /// <summary>
    /// Extracts id's from asset names.
    /// </summary>
    public static class AMSAssetNameIdExtractor
    {

        private static string Format => @"^reaction-asset-{0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";

        public static Guid GetId(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();
            if (!Regex.IsMatch(assetName, Format)) { throw new FormatException("Invalid asset name format for id extraction"); }
            var idSubString = assetName.Substring(15);
            return new Guid(idSubString);
        }

    }

}
