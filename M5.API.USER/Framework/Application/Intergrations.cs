using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using M5.API.USER.Framework.Domain.Events;
using MediatR;

namespace M5.API.USER.Framework.Application.Intergrations
{
    public class UserUpdateIntergrationEvent
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }

    public class UserUpdateIntergrationEventHandler : INotificationHandler<UserUpdateEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public UserUpdateIntergrationEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(UserUpdateEvent notification, CancellationToken cancellationToken)
        {
            _capPublisher.Publish("M5.API.USER.UserUpdate", new UserUpdateIntergrationEvent
            {
                Id = notification.User.Id,
                Phone = notification.User.Phone,
                Name = notification.User.Name
            });
            return Task.CompletedTask;
        }
    }
}
