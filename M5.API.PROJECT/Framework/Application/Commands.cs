using M5.API.PROJECT.Framework.Application.Intergrations;
using M5.API.PROJECT.Framework.Domain;
using M5.API.PROJECT.Framework.Domain.Aggregates;
using M5.API.PROJECT.Framework.Domain.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace M5.API.PROJECT.Framework.Application.Commands
{
    public class ProjectCreateCommand : IRequest<HandleResult>
    {
        public ProjectDTO ProjectDTO { get; set; }
    }

    public class ProjectCreateCommandHandler : IRequestHandler<ProjectCreateCommand, HandleResult>
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectCreateCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<HandleResult> Handle(ProjectCreateCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectDTO == null)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            if (string.IsNullOrWhiteSpace(request.ProjectDTO.Name))
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            if (request.ProjectDTO.UserId == Guid.Empty)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var project = _projectRepository.Create(new Project().Create(request.ProjectDTO));
            await _projectRepository.WorkUnit.SaveEntitiesAsync();
            var single = await _projectRepository.DetailAsync(project.Id);
            return HandleResult.Ok(content: single);
        }
    }

    public class MemberRelateUpdateCommand : UserUpdateIntergrationEvent, IRequest
    {
    }

    public class MemberRelateUpdateCommandHandler : AsyncRequestHandler<MemberRelateUpdateCommand>
    {
        private readonly IProjectRepository _projectRepository;

        public MemberRelateUpdateCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        protected async override Task Handle(MemberRelateUpdateCommand request, CancellationToken cancellationToken)
        {
            _projectRepository.MemberRelateUpdate(request.Id, request.Name);
            await _projectRepository.WorkUnit.SaveChangesAsync();
        }
    }
}
