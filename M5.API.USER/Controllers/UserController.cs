using M5.API.USER.Framework.Application.Commands;
using M5.API.USER.Framework.Application.Queries;
using M5.API.USER.Framework.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace M5.API.USER.Controllers
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IUserQuery _userQuery;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public UserController(IMediator mediator, IUserQuery userQuery, IHttpContextAccessor httpContextAccesor)
        {
            _mediator = mediator;
            _userQuery = userQuery;
            _httpContextAccesor = httpContextAccesor;
        }

        [HttpGet(nameof(Detail))]
        public async Task<IActionResult> Detail()
        {
            return ActionResult(await _userQuery.DetailAsync(Identity.Id));
        }

        [HttpPost(nameof(Signin))]
        public async Task<IActionResult> Signin([FromBody]UserDTO dto)
        {
            return ActionResult(await _mediator.Send(new UserCreateCommand { UserDTO = dto }));
        }

        [HttpPut(nameof(Update))]
        public async Task<IActionResult> Update([FromBody]UserDTO dto)
        {
            return ActionResult(await _mediator.Send(new UserUpdateCommand { UserDTO = dto }));
        }

        private async Task<string> GetTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}