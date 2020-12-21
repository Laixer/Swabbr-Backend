using Bogus;
using Swabbr.Testing.Extensions;
using Swabbr.Api.DataTransferObjects;

namespace Swabbr.Testing.Faker.DataTransferObjects
{
    /// <summary>
    ///     Faker for <see cref="ChangePasswordDto"/> data transfer object.
    /// </summary>
    public class ChangePasswordDtoFaker : Faker<ChangePasswordDto>
    {
        public ChangePasswordDtoFaker()
        {
            RuleFor(x => x.CurrentPassword, x => x.Random.Password(32));
            RuleFor(x => x.NewPassword, x => x.Random.Password(32));
        }
    }
}
