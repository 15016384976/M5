using MediatR;
using System;
using System.Collections.Generic;

namespace M5.API.PROJECT.Framework.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents
        {
            get
            {
                return _domainEvents?.AsReadOnly();
            }
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
