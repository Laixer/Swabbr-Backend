using System;
using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Core.Notifications
{
    public sealed class NotificationResponse<TResult> : NotificationResponse where TResult : class
    {
        public TResult Result { get; set; }

        public NotificationResponse()
        {
        }

        public new NotificationResponse<TResult> SetAsFailureResponse()
        {
            base.SetAsFailureResponse();
            return this;
        }

        public new NotificationResponse<TResult> AddErrorMessage(string errorMessage)
        {
            base.AddErrorMessage(errorMessage);
            return this;
        }
    }

    public class NotificationResponse
    {
        private bool _forcedFailedResponse;

        public bool CompletedWithSuccess => !ErrorMessages.Any() && !_forcedFailedResponse;
        public IList<string> ErrorMessages { get; private set; }

        public NotificationResponse()
        {
            ErrorMessages = new List<string>();
        }

        public NotificationResponse SetAsFailureResponse()
        {
            _forcedFailedResponse = true;
            return this;
        }

        public NotificationResponse AddErrorMessage(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
            return this;
        }

        public string FormattedErrorMessages => ErrorMessages.Any()
            ? ErrorMessages.Aggregate((prev, current) => prev + Environment.NewLine + current)
            : string.Empty;
    }
}
