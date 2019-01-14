﻿using MediatR;
using System.Linq;
using System.Threading.Tasks;
using M5.API.PROJECT.Framework.Domain;

namespace M5.API.PROJECT.Framework.Infrastructure
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, EFContext context)
        {
            var domainEntities = context.ChangeTracker
                                        .Entries<Entity>()
                                        .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents.Select(async (domainEvent) =>
            {
                await mediator.Publish(domainEvent);
            });

            await Task.WhenAll(tasks);
        }
    }
}
