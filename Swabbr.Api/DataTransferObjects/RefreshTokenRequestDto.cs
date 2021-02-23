namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     Dto for refreshing an expired jwt token.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        ///     Expired jwt token.
        /// </summary>
        public string ExpiredToken { get; set; }

        /// <summary>
        ///     Corresponding valid refresh token.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
