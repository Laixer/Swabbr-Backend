namespace Swabbr.Api.ViewModels
{
    public sealed class UserAuthenticationOutputModel
    {
        public string AccessToken { get; set; }
        public UserOutputModel User { get; set; }
        public UserSettingsOutputModel UserSettings { get; set; }
    }
}
