using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.Data;
using System;
using Xunit;

namespace Swabbr.Core.Tests.Notifications
{
    /// <summary>
    ///     Testscases for <see cref="NotificationFactory"/>.
    /// </summary>
    public class NotificationFactoryTests : IClassFixture<NotificationFactory>
    {
        private readonly INotificationFactory _notificationFactory;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotificationFactoryTests(NotificationFactory notificationFactory)
            => _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));

        [Fact]
        public void FollowedProfileVlogPostedIsCorrect()
        {
            // Arrange.
            var notifiedUserId = Guid.NewGuid();
            var vlogId = Guid.NewGuid();
            var vlogOwnerUserId = Guid.NewGuid();

            // Act.
            var notificationContext = _notificationFactory.BuildFollowedProfileVlogPosted(notifiedUserId, vlogId, vlogOwnerUserId);
            var data = notificationContext.Notification.Data as DataFollowedProfileVlogPosted;

            // Assert.
            Assert.IsType<DataFollowedProfileVlogPosted>(notificationContext.Notification.Data);
            Assert.Equal(vlogId, data.VlogId);
            Assert.Equal(vlogOwnerUserId, data.VlogOwnerUserId);
            Assert.True(notificationContext.HasUser);
            Assert.Equal(notificationContext.NotifiedUserId, notifiedUserId);
        }

        [Fact]
        public void RecordVlogIsCorrect()
        {
            // Arrange.
            var notifiedUserId = Guid.NewGuid();
            var vlogId = Guid.NewGuid();
            var requestMoment = DateTimeOffset.Now;
            var requestTimeout = TimeSpan.FromMinutes(5);

            // Act.
            var notificationContext = _notificationFactory.BuildVlogRecordRequest(notifiedUserId, vlogId, requestMoment, requestTimeout);
            var data = notificationContext.Notification.Data as DataVlogRecordRequest;

            // Assert.
            Assert.IsType<DataVlogRecordRequest>(notificationContext.Notification.Data);
            Assert.Equal(notifiedUserId, notificationContext.NotifiedUserId);
            Assert.Equal(vlogId, data.VlogId);
            Assert.Equal(requestMoment, data.RequestMoment);
            Assert.Equal(requestTimeout, data.RequestTimeout);
            Assert.True(notificationContext.HasUser);
            Assert.Equal(notificationContext.NotifiedUserId, notifiedUserId);
        }

        [Fact]
        public void VlogGainedLikeIsCorrect()
        {
            // Arrange.
            var notifiedUserId = Guid.NewGuid();
            var vlogId = Guid.NewGuid();
            var userThatLikedId = Guid.NewGuid();

            // Act.
            var notificationContext = _notificationFactory.BuildVlogGainedLike(notifiedUserId, vlogId, userThatLikedId);
            var data = notificationContext.Notification.Data as DataVlogGainedLike;

            // Assert.
            Assert.IsType<DataVlogGainedLike>(notificationContext.Notification.Data);
            Assert.Equal(notifiedUserId, notificationContext.NotifiedUserId);
            Assert.Equal(vlogId, data.VlogId);
            Assert.Equal(userThatLikedId, data.UserThatLikedId);
            Assert.True(notificationContext.HasUser);
            Assert.Equal(notificationContext.NotifiedUserId, notifiedUserId);
        }

        [Fact]
        public void VlogNewReactionIsCorrect()
        {
            // Arrange.
            var notifiedUserId = Guid.NewGuid();
            var vlogId = Guid.NewGuid();
            var reactionId = Guid.NewGuid();

            // Act.
            var notificationContext = _notificationFactory.BuildVlogNewReaction(notifiedUserId, vlogId, reactionId);
            var data = notificationContext.Notification.Data as DataVlogNewReaction;

            // Assert.
            Assert.IsType<DataVlogNewReaction>(notificationContext.Notification.Data);
            Assert.Equal(notifiedUserId, notificationContext.NotifiedUserId);
            Assert.Equal(vlogId, data.VlogId);
            Assert.Equal(reactionId, data.ReactionId);
            Assert.True(notificationContext.HasUser);
            Assert.Equal(notificationContext.NotifiedUserId, notifiedUserId);
        }
    }
}
