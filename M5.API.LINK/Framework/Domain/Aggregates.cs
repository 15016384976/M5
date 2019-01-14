using System;
using System.Collections.Generic;

namespace M5.API.LINK.Framework.Domain.Aggregates
{
    public class Link : Entity, IAggregateRoot
    {
        public Guid UserId { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }

    public class Contact : Entity
    {
        public Guid LinkId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
