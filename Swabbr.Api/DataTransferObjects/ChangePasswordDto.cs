namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for changing a password.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        ///     Old user password.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        ///     New user password.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
