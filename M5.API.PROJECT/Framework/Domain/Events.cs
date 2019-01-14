using M5.API.PROJECT.Framework.Domain.Aggregates;
using MediatR;

namespace M5.API.PROJECT.Framework.Domain.Events
{
    public class ProjectCreateEvent : INotification
    {
        public Project Project { get; set; }
    }
}
