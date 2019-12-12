using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    public class IdentityUserTableEntity : TableEntity
    {
        public Guid UserId { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string NormalizedUserName { get; set; }
        
        public string NormalizedEmail { get; set; }
        
        public DateTimeOffset? LockoutEnd { get; set; }
        
        public bool LockoutEnabled { get; set; }
        
        public bool EmailConfirmed { get; set; }
        
        public string Email { get; set; }
        
        public string ConcurrencyStamp { get; set; }
        
        public int AccessFailedCount { get; set; }
        
        public bool TwoFactorEnabled { get; set; }
    }
}
