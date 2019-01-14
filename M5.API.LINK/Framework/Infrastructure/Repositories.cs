using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M5.API.LINK.Framework.Domain;
using M5.API.LINK.Framework.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace M5.API.LINK.Framework.Infrastructure.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly EFContext _context;

        public IWorkUnit WorkUnit
        {
            get
            {
                return _context;
            }
        }

        public LinkRepository(EFContext context)
        {
            _context = context;
        }

        public async Task<LinkDTO> DetailAsync(Guid userId)
        {
            var link = await _context.Links
                                     .AsNoTracking()
                                     .Include(v => v.Contacts)
                                     .SingleOrDefaultAsync(v => v.UserId == userId);

            LinkDTO dto = null;

            if (link == null)
                return dto;

            dto = new LinkDTO
            {
                Id = link.Id,
                UserId = link.UserId,
                Contacts = link.Contacts?.Select(v => new ContactDTO
                {
                    Id = v.Id,
                    LinkId = v.LinkId,
                    UserId = v.UserId,
                    Name = v.Name
                }).ToList() ?? new List<ContactDTO>()
            };

            return dto;
        }

        public void MemberRelateUpdate(Guid userId, string name)
        {
            _context.Contacts.Where(v => v.UserId == userId).ToList().ForEach(v => v.Name = name);
        }
    }
}
