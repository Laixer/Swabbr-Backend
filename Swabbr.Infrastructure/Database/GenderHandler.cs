using Dapper;
using Swabbr.Core.Enums;
using System;
using System.Data;

namespace Swabbr.Infrastructure.Database
{

    public sealed class GenderHandler : SqlMapper.TypeHandler<Gender?>
    {
        public override Gender? Parse(object value)
        {
            if (value == null) { return null; }
            throw new NotImplementedException();
        }

        public override void SetValue(IDbDataParameter parameter, Gender? value)
        {
            throw new NotImplementedException();
        }
    }
}
