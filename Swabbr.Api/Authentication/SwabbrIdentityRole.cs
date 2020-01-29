namespace Swabbr.Api.Authentication
{
    public class SwabbrIdentityRole
    {
        /// <summary>
        /// Id of the role.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// The name which the role represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The normalized name of the role.
        /// </summary>
        public string NormalizedName { get; set; }
    }
}