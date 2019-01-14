using System;

namespace M5.API.LINK.Framework.Application.Intergrations
{
    public class UserUpdateIntergrationEvent // from M5.API.USER
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
}
