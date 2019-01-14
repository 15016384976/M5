using DotNetCore.CAP;
using M5.API.PROJECT.Framework.Domain.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M5.API.PROJECT.Framework.Application.Intergrations
{
    public class ProjectCreateIntergrationEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }

    public class ProjectCreateIntergrationEventHandler : INotificationHandler<ProjectCreateEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public ProjectCreateIntergrationEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public Task Handle(ProjectCreateEvent notification, CancellationToken cancellationToken)
        {
            _capPublisher.Publish("M5.API.PROJECT.ProjectCreate", new ProjectCreateIntergrationEvent
            {
                Id = notification.Project.Id,
                Name = notification.Project.Name,
                UserId = notification.Project.UserId
            });
            return Task.CompletedTask;
        }
    }

    public class UserUpdateIntergrationEvent // from M5.API.USER
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
}
