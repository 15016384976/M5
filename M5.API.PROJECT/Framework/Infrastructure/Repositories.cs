using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M5.API.PROJECT.Framework.Domain;
using M5.API.PROJECT.Framework.Domain.Aggregates;
using M5.API.PROJECT.Framework.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace M5.API.PROJECT.Framework.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly EFContext _context;

        public IWorkUnit WorkUnit
        {
            get
            {
                return _context;
            }
        }

        public ProjectRepository(EFContext context)
        {
            _context = context;
        }

        public async Task<ProjectDTO> SingleAsync(Guid id)
        {
            var project = await _context.Projects
                                        .AsNoTracking()
                                        .Include(v => v.Properties)
                                        .Include(v => v.Members)
                                        .SingleOrDefaultAsync(v => v.Id == id);

            ProjectDTO dto = null;

            if (project == null)
                return dto;

            dto = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                UserId = project.UserId,
                Properties = project.Properties?.Select(v => new PropertyDTO
                {
                    ProjectId = v.ProjectId,
                    Key = v.Key,
                    Value = v.Value,
                    Title = v.Title
                }).ToList() ?? new List<PropertyDTO>(),
                Members = project.Members?.Select(v => new MemberDTO
                {
                    Id = v.Id,
                    ProjectId = v.ProjectId,
                    UserId = v.UserId,
                    Name = v.Name
                }).ToList() ?? new List<MemberDTO>()
            };

            return dto;
        }

        public async Task<ProjectDTO> DetailAsync(Guid id)
        {
            var project = await _context.Projects
                                        .AsNoTracking()
                                        .Include(v => v.Properties)
                                        .Include(v => v.Members)
                                        .SingleOrDefaultAsync(v => v.Id == id);

            ProjectDTO dto = null;

            if (project == null)
                return dto;

            dto = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                UserId = project.UserId,
                Properties = project.Properties?.Select(v => new PropertyDTO
                {
                    ProjectId = v.ProjectId,
                    Key = v.Key,
                    Value = v.Value,
                    Title = v.Title
                }).ToList() ?? new List<PropertyDTO>(),
                Members = project.Members?.Select(v => new MemberDTO
                {
                    Id = v.Id,
                    ProjectId = v.ProjectId,
                    UserId = v.UserId,
                    Name = v.Name
                }).ToList() ?? new List<MemberDTO>()
            };

            return dto;
        }

        public Project Create(Project project)
        {
            return _context.Add(project).Entity;
        }

        public void MemberRelateUpdate(Guid userId, string name)
        {
            _context.Members.Where(v => v.UserId == userId).ToList().ForEach(v => v.Name = name);
        }
    }
}
