using MediatR;
using Microsoft.EntityFrameworkCore;
using AcademyIO.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIO.ManagementCourses.Data;

public static class MediatorExtension
{
    [ExcludeFromCodeCoverage]
    public static async Task PublishDomainEvents(this IMediator mediator, DbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities.SelectMany(x => x.Notifications).ToList();

        domainEntities.ForEach(entity => entity.CleanEvents());

        var tasks = domainEvents.Select(async (domainEvent) =>
        {
            await mediator.Publish(domainEvent);
        });

        await Task.WhenAll(tasks);
    }
}