using System;
using System.Threading.Tasks;
using M5.API.USER.Framework.Domain.Aggregates;
using M5.API.USER.Framework.Domain.Dtos;

namespace M5.API.USER.Framework.Domain
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IWorkUnit WorkUnit { get; }
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<UserDTO> SingleAsync(string phone);
        Task<UserDTO> DetailAsync(Guid id);
        User Create(User user);
        User Update(User user);
    }
}
