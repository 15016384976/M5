using M5.API.USER.Framework.Domain.Aggregates;
using MediatR;

namespace M5.API.USER.Framework.Domain.Events
{
    public class UserUpdateEvent : INotification
    {
        public User User { get; set; }
    }
}
