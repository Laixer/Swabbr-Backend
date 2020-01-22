using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Errors;

namespace Swabbr.Api.Extensions
{
    public static class ControllerExtensions
    {
        public static ErrorMessage Error(this ControllerBase controller, int errorCode, string message)
        {
            return new ErrorMessage (errorCode, message);
        }
    }
}
