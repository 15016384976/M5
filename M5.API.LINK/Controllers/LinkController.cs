using DotNetCore.CAP;
using M5.API.LINK.Framework.Application.Commands;
using M5.API.LINK.Framework.Application.Intergrations;
using M5.API.LINK.Framework.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace M5.API.LINK.Controllers
{
    [Route("api/[controller]")]
    public class LinkController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILinkQuery _linkQuery;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public LinkController(IMediator mediator, ILinkQuery linkQuery, IHttpContextAccessor httpContextAccesor)
        {
            _mediator = mediator;
            _linkQuery = linkQuery;
            _httpContextAccesor = httpContextAccesor;
        }

        [HttpGet(nameof(Detail))]
        public async Task<IActionResult> Detail()
        {
            return ActionResult(await _linkQuery.DetailAsync(Identity.Id));
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
