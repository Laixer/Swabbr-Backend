using Microsoft.AspNet.Identity;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Swabbr.Identity
{
    public class AzureTableIdentityUser : TableEntity, IUser
    {
        public string Id => UserId;

        public string UserName { get => Email; set => Email = value; }

        public string UserId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int FailedLogIns { get; set; }
        public DateTimeOffset LockOutEndDate { get; set; }

        public IList<string> Roles { get; set; }
        public IList<string> Claims { get; set; }
    }
}