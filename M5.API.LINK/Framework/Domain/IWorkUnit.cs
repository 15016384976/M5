using System;
using System.Threading;
using System.Threading.Tasks;

namespace M5.API.LINK.Framework.Domain
{
    public interface IWorkUnit : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
