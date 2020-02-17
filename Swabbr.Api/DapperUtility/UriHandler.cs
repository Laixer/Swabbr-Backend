using Dapper;
using System;
using System.Data;

namespace Swabbr.Api.DapperUtility
{

    /// <summary>
    /// Implements custom type handling functionality for Dapper regarding <see cref="Uri"/> objects.
    /// </summary>
    public class UriHandler : SqlMapper.TypeHandler<Uri>
    {

        public override Uri Parse(object value)
        {
            if (value == null) { return null; } // TODO Do we want this?
            return new Uri(value.ToString()); // TODO Unsafe!
        }

        public override void SetValue(IDbDataParameter parameter, Uri value)
        {
            parameter.Value = value.AbsoluteUri;
        }

    }

}

