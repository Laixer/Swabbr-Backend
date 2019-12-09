using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Api.Test.Models;
using System;
using System.Collections.Generic;

namespace Swabbr.Api.Test.Helpers
{
    public class AzureUserManager : UserManager<AzureTableUser>
    {
        public AzureUserManager(
            IUserStore<AzureTableUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AzureTableUser> passwordHasher,
            IEnumerable<IUserValidator<AzureTableUser>> userValidators,
            IEnumerable<IPasswordValidator<AzureTableUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AzureTableUser>> logger)
    : base(
        store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
        services, logger)
        {
        }
    }
}