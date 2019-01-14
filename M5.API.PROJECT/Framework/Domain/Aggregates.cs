using System;
using System.Collections.Generic;
using System.Linq;
using M5.API.PROJECT.Framework.Domain.Dtos;
using M5.API.PROJECT.Framework.Domain.Events;

namespace M5.API.PROJECT.Framework.Domain.Aggregates
{
    public class Project : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();
        public List<Member> Members { get; set; } = new List<Member>();

        public Project Create(ProjectDTO dto)
        {
            Id = Guid.NewGuid();
            Name = dto.Name;
            UserId = dto.UserId;
            Properties = dto.Properties.Any() ? dto.Properties.Select(v => new Property
            {
                ProjectId = Id,
                Key = v.Key,
                Value = v.Value,
                Title = v.Title
            }).ToList() : new List<Property>();

            AddDomainEvent(new ProjectCreateEvent { Project = this });

            return this;
        }
    }

    public class Property : ValueObject
    {
        public Guid ProjectId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Key;
            yield return Value;
            yield return Title;
        }
    }

    public class Member : Entity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
