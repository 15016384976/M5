using M5.API.USER.Framework.Domain;
using M5.API.USER.Framework.Domain.Aggregates;
using M5.API.USER.Framework.Domain.Dtos;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace M5.API.USER.Framework.Application.Commands
{
    public class UserCreateCommand : IRequest<HandleResult>
    {
        public UserDTO UserDTO { get; set; }
    }

    public class UserCreateCommandHandler : IRequestHandler<UserCreateCommand, HandleResult>
    {
        private readonly IUserRepository _userRepository;

        public UserCreateCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandleResult> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.UserDTO == null)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            if (string.IsNullOrWhiteSpace(request.UserDTO.Phone))
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var single = await _userRepository.SingleAsync(request.UserDTO.Phone);
            if (single == null)
            {
                var user = _userRepository.Create(new User().Create(request.UserDTO));
                await _userRepository.WorkUnit.SaveEntitiesAsync();
                single = await _userRepository.DetailAsync(user.Id);
            }
            return HandleResult.Ok(content: single);
        }
    }

    public class UserUpdateCommand : IRequest<HandleResult>
    {
        public UserDTO UserDTO { get; set; }
    }

    public class UserUpdateCommandHandler : IRequestHandler<UserUpdateCommand, HandleResult>
    {
        private readonly IUserRepository _userRepository;

        public UserUpdateCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandleResult> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            if (request.UserDTO == null)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var single = await _userRepository.DetailAsync(request.UserDTO.Id);
            if (single == null)
                return HandleResult.NotFound(errorCode: "error code", errorMessage: "error message");

            var user = _userRepository.Update(new User().Update(request.UserDTO));
            await _userRepository.WorkUnit.SaveEntitiesAsync();
            single = await _userRepository.DetailAsync(user.Id);
            return HandleResult.Ok(content: single);
        }
    }
}