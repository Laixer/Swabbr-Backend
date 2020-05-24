using Dapper;
using System;
using System.Data;

namespace Swabbr.Infrastructure.Database
{

    /// <summary>
    /// Implements custom type handling functionality for Dapper regarding <see cref="Uri"/> objects.
    /// </summary>
    public sealed class UriHandler : SqlMapper.TypeHandler<Uri>
    {

        public override Uri Parse(object value)
        {
            if (value == null) { return null; } // TODO Do we want this?
            try
            {
                var toString = value.ToString();
                var builder = new UriBuilder(value.ToString());
                return builder.Uri;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error while parsing {value} to URI", e);
            }
        }

        public override void SetValue(IDbDataParameter parameter, Uri value)
        {
            if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            parameter.Value = value.AbsoluteUri;
        }

    }

}

