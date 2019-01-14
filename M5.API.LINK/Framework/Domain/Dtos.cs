using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace M5.API.LINK.Framework.Domain.Dtos
{
    public class LinkDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<ContactDTO> Contacts { get; set; } = new List<ContactDTO>();
    }

    public class ContactDTO
    {
        public Guid Id { get; set; }
        public Guid LinkId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
