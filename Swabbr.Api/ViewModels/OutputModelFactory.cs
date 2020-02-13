using Swabbr.Core.Entities;

namespace Swabbr.Api.ViewModels
{
    public partial class UserOutputModel
    {
        public static UserOutputModel Parse(SwabbrUser user) => new UserOutputModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.BirthDate,
            Country = user.Country,
            Gender = user.Gender,
            IsPrivate = user.IsPrivate,
            Nickname = user.Nickname,
            ProfileImageUrl = user.ProfileImageUrl.ToString(), // TODO THOMAS Check this
            Timezone = user.Timezone
        };
    }
}
