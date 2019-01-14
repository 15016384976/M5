using M5.API.LINK.Framework.Domain;
using System;
using System.Threading.Tasks;

namespace M5.API.LINK.Framework.Application.Queries
{
    public interface ILinkQuery
    {
        Task<HandleResult> DetailAsync(Guid userId);
    }

    public class LinkQuery : ILinkQuery
    {
        private readonly ILinkRepository _linkRepository;

        public LinkQuery(ILinkRepository linkRepository)
        {
            _linkRepository = linkRepository;
        }

        public async Task<HandleResult> DetailAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var detail = await _linkRepository.DetailAsync(userId);
            if (detail == null)
                return HandleResult.NotFound(errorCode: "error code", errorMessage: "error message");

            return HandleResult.Ok(content: detail);
        }
    }
}
