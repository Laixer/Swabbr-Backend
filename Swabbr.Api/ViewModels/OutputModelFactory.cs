using Swabbr.Core.Entities;

namespace Swabbr.Api.ViewModels
{
    public partial class UserOutputModel
    {
        public static UserOutputModel Parse(User user) => new UserOutputModel
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Country = user.Country,
            Gender = user.Gender,
            IsPrivate = user.IsPrivate,
            Nickname = user.Nickname,
            ProfileImageUrl = user.ProfileImageUrl,
            Timezone = user.Timezone
        };
    }
}
