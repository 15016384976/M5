using M5.API.LINK.Framework.Application.Intergrations;
using M5.API.LINK.Framework.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace M5.API.LINK.Framework.Application.Commands
{
    public class MemberRelateUpdateCommand : UserUpdateIntergrationEvent, IRequest
    {
    }

    public class MemberRelateUpdateCommandHandler : AsyncRequestHandler<MemberRelateUpdateCommand>
    {
        private readonly ILinkRepository _linkRepository;

        public MemberRelateUpdateCommandHandler(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        protected async override Task Handle(MemberRelateUpdateCommand request, CancellationToken cancellationToken)
        {
            _linkRepository.MemberRelateUpdate(request.Id, request.Name);
            await _linkRepository.WorkUnit.SaveChangesAsync();
        }
    }
}
