using DotNetCore.CAP;
using M5.API.PROJECT.Framework.Application.Commands;
using M5.API.PROJECT.Framework.Application.Intergrations;
using M5.API.PROJECT.Framework.Application.Queries;
using M5.API.PROJECT.Framework.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace M5.API.PROJECT.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IProjectQuery _projectQuery;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public ProjectController(IMediator mediator, IProjectQuery projectQuery, IHttpContextAccessor httpContextAccesor)
        {
            _mediator = mediator;
            _projectQuery = projectQuery;
            _httpContextAccesor = httpContextAccesor;
        }

        [HttpGet(nameof(Detail))]
        public async Task<IActionResult> Detail([FromQuery]Guid id)
        {
            return ActionResult(await _projectQuery.DetailAsync(id));
        }

        [HttpPost(nameof(Create))]
        public async Task<IActionResult> Create([FromBody]ProjectDTO dto)
        {
            return ActionResult(await _mediator.Send(new ProjectCreateCommand { ProjectDTO = dto }));
        }

        private async Task<string> GetTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }

        [CapSubscribe("M5.API.USER.UserUpdate")]
        private async Task UserUpdateHandleAsync(UserUpdateIntergrationEvent ev)
        {
            await _mediator.Send(new MemberRelateUpdateCommand { Id = ev.Id, Phone = ev.Phone, Name = ev.Name });
        }
    }
}
