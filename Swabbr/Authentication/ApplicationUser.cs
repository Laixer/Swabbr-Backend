using Microsoft.AspNet.Identity;

namespace Swabbr.Api.Authentication
{
    public class ApplicationUser : Swabbr.Core.Entities.User, IUser
    {
        public string Id => UserId.ToString();

        public string UserName { get => Email; set => Email = value; }
    }
}
