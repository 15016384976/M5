using System;
using System.Threading.Tasks;
using M5.API.PROJECT.Framework.Domain.Aggregates;
using M5.API.PROJECT.Framework.Domain.Dtos;

namespace M5.API.PROJECT.Framework.Domain
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IWorkUnit WorkUnit { get; }
    }

    public interface IProjectRepository : IRepository<Project>
    {
        Task<ProjectDTO> SingleAsync(Guid id);
        Task<ProjectDTO> DetailAsync(Guid id);
        Project Create(Project project);
        void MemberRelateUpdate(Guid userId, string name);
    }
}
