using Dapper;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.DapperUtility
{
    public class FollowRequestStatusHandler : SqlMapper.TypeHandler<FollowRequestStatus>
    {
        public override FollowRequestStatus Parse(object value)
        {
            var result = (FollowRequestStatus)value;
            return result;
        }

        public override void SetValue(IDbDataParameter parameter, FollowRequestStatus value)
        {
            parameter.Value = value;
        }
    }
}
