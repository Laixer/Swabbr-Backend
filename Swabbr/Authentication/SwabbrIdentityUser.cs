﻿using Swabbr.Infrastructure.Data.Entities;
using System;

namespace Swabbr.Api.Authentication
{
    public class SwabbrIdentityUser : UserTableEntity
    {
        public string PasswordHash { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string ConcurrencyStamp { get; set; }

        public int AccessFailedCount { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}