using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M5.API.USER.Framework.Domain;
using M5.API.USER.Framework.Domain.Aggregates;
using M5.API.USER.Framework.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace M5.API.USER.Framework.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EFContext _context;

        public IWorkUnit WorkUnit
        {
            get
            {
                return _context;
            }
        }

        public UserRepository(EFContext context)
        {
            _context = context;
        }

        public async Task<UserDTO> SingleAsync(string phone)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .Include(v => v.Properties)
                                     .Include(v => v.Labels)
                                     .SingleOrDefaultAsync(v => v.Phone == phone);

            UserDTO dto = null;

            if (user == null)
                return dto;

            dto = new UserDTO
            {
                Id = user.Id,
                Phone = user.Phone,
                Name = user.Name,
                Properties = user.Properties.Any() ? user.Properties.Select(v => new PropertyDTO
                {
                    UserId = v.UserId,
                    Key = v.Key,
                    Value = v.Value,
                    Title = v.Title
                }).ToList() : new List<PropertyDTO>(),
                Labels = user.Labels.Any() ? user.Labels.Select(v => new LabelDTO
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    Title = v.Title
                }).ToList() : new List<LabelDTO>()
            };

            return dto;
        }

        public async Task<UserDTO> DetailAsync(Guid id)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .Include(v => v.Properties)
                                     .Include(v => v.Labels)
                                     .SingleOrDefaultAsync(v => v.Id == id);

            UserDTO dto = null;

            if (user == null)
                return dto;

            dto = new UserDTO
            {
                Id = user.Id,
                Phone = user.Phone,
                Name = user.Name,
                Properties = user.Properties.Any() ? user.Properties.Select(v => new PropertyDTO
                {
                    UserId = v.UserId,
                    Key = v.Key,
                    Value = v.Value,
                    Title = v.Title
                }).ToList() : new List<PropertyDTO>(),
                Labels = user.Labels.Any() ? user.Labels.Select(v => new LabelDTO
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    Title = v.Title
                }).ToList() : new List<LabelDTO>()
            };

            return dto;
        }

        public User Create(User user)
        {
            return _context.Add(user).Entity;
        }

        public User Update(User user)
        {
            var originalProperties = _context.Properties.AsNoTracking().Where(v => v.UserId == user.Id).ToList();
            var originalLabels = _context.Labels.AsNoTracking().Where(v => v.UserId == user.Id).ToList();

            _context.Properties.RemoveRange(originalProperties.ToArray());
            _context.Labels.RemoveRange(originalLabels.ToArray());

            var properties = user.Properties.Any() ? user.Properties.Select(v => new Property
            {
                UserId = user.Id,
                Key = v.Key,
                Value = v.Value,
                Title = v.Title
            }).ToList() : new List<Property>();

            var labels = user.Labels.Any() ? user.Labels.Select(v => new Label
            {
                UserId = user.Id,
                Title = v.Title
            }).ToList() : new List<Label>();

            _context.Properties.AddRange(properties.ToArray());
            _context.Labels.AddRange(labels.ToArray());

            user.Properties = new List<Property>();
            user.Labels = new List<Label>();

            return _context.Users.Update(user).Entity;
        }
    }
}
