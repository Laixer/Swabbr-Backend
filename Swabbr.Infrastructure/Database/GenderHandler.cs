using Dapper;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
