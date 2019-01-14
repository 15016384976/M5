using System;
using System.Threading.Tasks;
using M5.API.LINK.Framework.Domain.Aggregates;
using M5.API.LINK.Framework.Domain.Dtos;

namespace M5.API.LINK.Framework.Domain
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IWorkUnit WorkUnit { get; }
    }

    public interface ILinkRepository : IRepository<Link>
    {
        Task<LinkDTO> DetailAsync(Guid userId);
        void MemberRelateUpdate(Guid userId, string name);
    }
}
