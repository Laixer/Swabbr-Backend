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
                    first_name,
                    last_name,
                    nickname,
                    is_private,
                    profile_image_url,
                    gender,
                    timezone,
                    birth_date,
                    country
                ) VALUES (
                    @Email,
                    @NormalizedEmail,
                    @EmailConfirmed,
                    @PasswordHash,
                    @SecurityStamp,
                    @PhoneNumber,
                    @LockoutEnd,
                    @FirstName,
                    @LastName,
                    @Nickname,
                    @IsPrivate,
                    @ProfileImageUrl,
                    CAST (@GenderText AS gender),
                    @Timezone,
                    @BirthDate,
                    @Country
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
                       given_name = @GivenName,
                       last_name = @LastName,
                       profile_image_url = @ProfileImageUrl,
                       job_title = @JobTitle,
                       password_hash = @PasswordHash,
                       phone_number = @PhoneNumber,
                       phone_number_confirmed = @PhoneNumberConfirmed,
                       two_factor_enabled = @TwoFactorEnabled,
                       lockout_end = @LockoutEnd,
                       lockout_enabled = @LockoutEnabled,
                       access_failed_count = @AccessFailedCount,
                       given_name = @GivenName,
                       last_name = @LastName,
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
