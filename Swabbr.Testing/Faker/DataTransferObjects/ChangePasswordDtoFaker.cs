using Bogus;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Testing.Extensions;

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
