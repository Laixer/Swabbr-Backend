using Laixer.Identity.Dapper;
using System;

namespace Swabbr.Api.Authentication
{

    /// <summary>
    /// Contains custom sql statements so we can use dapper with postgresql
    /// to execute our identity functionality.
    /// </summary>
    public class IdentityQueryRepository : ICustomQueryRepository
    {

        /// <summary>
        /// Configure custom queries.
        /// </summary>
        /// <param name="queryRepository">Existing query repository.</param>
        public void Configure(IQueryRepository queryRepository)
        {
            if (queryRepository == null) { throw new ArgumentNullException(nameof(queryRepository)); }

            queryRepository.CreateAsync = $@"
                INSERT INTO public.user (
                    email,
                    normalized_email,
                    email_confirmed,
                    password_hash,
                    security_stamp,
                    phone_number,
                    lockout_end,
                    nickname
                ) VALUES (
                    @Email,
                    @NormalizedEmail,
                    @EmailConfirmed,
                    @PasswordHash,
                    @SecurityStamp,
                    @PhoneNumber,
                    @LockoutEnd,
                    @Nickname
                ) RETURNING id";

            queryRepository.FindByNameAsync = @"
                SELECT *
                FROM public.user
                WHERE normalized_email=@NormalizedUserName";

            queryRepository.UpdateAsync = @"
                UPDATE public.user
                SET    email = @Email,
                       normalized_email = @NormalizedEmail,
                       email_confirmed = @EmailConfirmed,
                       password_hash = @PasswordHash,
                       phone_number = @PhoneNumber,
                       phone_number_confirmed = @PhoneNumberConfirmed,
                       two_factor_enabled = @TwoFactorEnabled,
                       lockout_end = @LockoutEnd,
                       lockout_enabled = @LockoutEnabled,
                       access_failed_count = @AccessFailedCount
                WHERE  id = @Id";

            queryRepository.FindByIdAsync = $@"
                SELECT *
                FROM public.user
                WHERE Id=@Id::uuid
                LIMIT 1";

            queryRepository.AddToRoleAsync = $@"
                UPDATE public.user
                SET role = @Role
                WHERE id=@Id";

            queryRepository.GetRolesAsync = $@"
                SELECT role
                FROM public.user
                WHERE id=@Id";

            queryRepository.GetUsersInRoleAsync = $@"
                SELECT *
                FROM public.user
                WHERE role=@Role";

            queryRepository.RemoveFromRoleAsync = $@"
                UPDATE public.user
                SET role = 'user'
                WHERE id=@Id";
        }

    }

}
