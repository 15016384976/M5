using System;
using System.Collections.Generic;

namespace M5.API.USER.Framework.Domain.Dtos
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public List<PropertyDTO> Properties { get; set; } = new List<PropertyDTO>();
        public List<LabelDTO> Labels { get; set; } = new List<LabelDTO>();
    }

    public class PropertyDTO
    {
        public Guid UserId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }

    public class LabelDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
    }
}
