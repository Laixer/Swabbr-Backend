﻿namespace Swabbr.Api.Authentication
{
    public class AppRole
    {
        /// <summary>
        /// Id of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The name which the role represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The normalized name which the role represents.
        /// </summary>
        public string NormalizedName { get; set; }
    }
}