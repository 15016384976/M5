using M5.API.USER.Framework.Domain;
using System;
using System.Threading.Tasks;

namespace M5.API.USER.Framework.Application.Queries
{
    public interface IUserQuery
    {
        Task<HandleResult> SingleAsync(string phone);
        Task<HandleResult> DetailAsync(Guid id);
    }

    public class UserQuery : IUserQuery
    {
        private readonly IUserRepository _userRepository;

        public UserQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandleResult> SingleAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var single = await _userRepository.SingleAsync(phone);
            if (single == null)
                return HandleResult.NotFound(errorCode: "error code", errorMessage: "error message");

            return HandleResult.Ok(content: single);
        }

        public async Task<HandleResult> DetailAsync(Guid id)
        {
            if (id == Guid.Empty)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var detail = await _userRepository.DetailAsync(id);
            if (detail == null)
                return HandleResult.NotFound(errorCode: "error code", errorMessage: "error message");

            return HandleResult.Ok(content: detail);
        }
    }
}
