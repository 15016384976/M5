using M5.API.USER.Framework.Domain.Dtos;
using M5.API.USER.Framework.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace M5.API.USER.Framework.Domain.Aggregates
{
    public class User : Entity, IAggregateRoot
    {
        public string Phone { get; set; }
        public string Name { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();
        public List<Label> Labels { get; set; } = new List<Label>();

        public User Create(UserDTO dto)
        {
            Id = Guid.NewGuid();
            Phone = dto.Phone;
            Name = dto.Name ?? "-";
            Properties = dto.Properties.Any() ? dto.Properties.Select(v => new Property
            {
                UserId = v.UserId,
                Key = v.Key,
                Value = v.Value,
                Title = v.Title
            }).ToList() : new List<Property>();
            Labels = dto.Labels.Any() ? dto.Labels.Select(v => new Label
            {
                UserId = v.UserId,
                Title = v.Title
            }).ToList() : new List<Label>();

            return this;
        }

        public User Update(UserDTO dto)
        {
            Id = dto.Id;
            Phone = dto.Phone;
            Name = dto.Name;
            Properties = dto.Properties.Any() ? dto.Properties.Select(v => new Property
            {
                UserId = Id,
                Key = v.Key,
                Value = v.Value,
                Title = v.Title
            }).ToList() : new List<Property>();
            Labels = dto.Labels.Any() ? dto.Labels.Select(v => new Label
            {
                UserId = Id,
                Title = v.Title
            }).ToList() : new List<Label>();

            AddDomainEvent(new UserUpdateEvent { User = this });

            return this;
        }
    }

    public class Property : ValueObject
    {
        public Guid UserId { get; set; }
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

    public class Label : Entity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
    }
}
