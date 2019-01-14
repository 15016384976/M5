using System;
using System.Collections.Generic;

namespace M5.API.PROJECT.Framework.Domain.Dtos
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public List<PropertyDTO> Properties { get; set; } = new List<PropertyDTO>();
        public List<MemberDTO> Members { get; set; } = new List<MemberDTO>();
    }

    public class PropertyDTO
    {
        public Guid ProjectId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }

    public class MemberDTO
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
