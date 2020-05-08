using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Input model for changing the user password.
    /// </summary>
    public sealed class UserChangePasswordInputModel
    {

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

    }

}
