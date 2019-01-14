using M5.API.PROJECT.Framework.Domain;
using System;
using System.Threading.Tasks;

namespace M5.API.PROJECT.Framework.Application.Queries
{
    public interface IProjectQuery
    {
        Task<HandleResult> DetailAsync(Guid id);
    }

    public class ProjectQuery : IProjectQuery
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectQuery(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<HandleResult> DetailAsync(Guid id)
        {
            if (id == Guid.Empty)
                return HandleResult.BadRequest(errorCode: "error code", errorMessage: "error message");

            var detail = await _projectRepository.DetailAsync(id);
            if (detail == null)
                return HandleResult.NotFound(errorCode: "error code", errorMessage: "error message");

            return HandleResult.Ok(content: detail);
        }
    }
}
