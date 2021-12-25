using Laixer.Identity.Dapper;
using System;

namespace Swabbr.Api.Authentication
{
    /// <summary>
    ///     Contains custom sql statements so we can use dapper 
    ///     with postgresql to execute our identity functionality.
    /// </summary>
    public class IdentityQueryRepository : ICustomQueryRepository
    {
        /// <summary>
        ///     Configure custom queries.
        /// </summary>
        /// <param name="queryRepository">Existing query repository.</param>
        public void Configure(IQueryRepository queryRepository)
        {
            if (queryRepository is null) 
            { 
                throw new ArgumentNullException(nameof(queryRepository)); 
            }

            queryRepository.CreateAsync = $@"
                INSERT INTO application.user (
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

            queryRepository.DeleteAsync = @"
                UPDATE      application.user AS u
                SET         user_status = 'deleted'
                WHERE       u.id = @Id";

            // TODO Is this correct? When will we use this?
            queryRepository.FindByNameAsync = @"
                SELECT *
                FROM application.user_up_to_date
                WHERE normalized_email=@NormalizedUserName";

            queryRepository.FindByEmailAsync = @"
                SELECT *
                FROM application.user_up_to_date
                WHERE normalized_email=@NormalizedEmail";

            queryRepository.UpdateAsync = @"
                UPDATE application.user_up_to_date
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
                FROM application.user_up_to_date
                WHERE Id=@Id::uuid
                LIMIT 1";

            queryRepository.AddToRoleAsync = $@"
                UPDATE application.user_up_to_date
                SET role = @Role
                WHERE id=@Id";

            queryRepository.GetRolesAsync = $@"
                SELECT role
                FROM application.user_up_to_date
                WHERE id=@Id";

            queryRepository.GetUsersInRoleAsync = $@"
                SELECT *
                FROM application.user_up_to_date
                WHERE role=@Role";

            queryRepository.RemoveFromRoleAsync = $@"
                UPDATE application.user_up_to_date
                SET role = 'user'
                WHERE id=@Id";
        }
    }
}
