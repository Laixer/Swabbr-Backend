using Microsoft.AspNet.Identity;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Swabbr.Identity
{
    // TODO Turn "Swabbr.Infrastructure.Data.Entities.UserEntity" table entity into this?

    public class AzureTableIdentityUser : TableEntity, IUser
    {
        public string Id => UserId.ToString();

        public string UserName { get => Email; set => Email = value; }

        public Guid UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int FailedLogIns { get; set; }
        public DateTimeOffset LockOutEndDate { get; set; }

        public IList<string> Roles { get; set; }
        public IList<string> Claims { get; set; }
    }
}