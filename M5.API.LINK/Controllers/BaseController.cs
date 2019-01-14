using M5.API.LINK.Framework.Domain;
using M5.API.LINK.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace M5.API.LINK.Controllers
{
    public class BaseController : Controller
    {
        protected MIdentity Identity
        {
            get
            {
                return new MIdentity
                {
                    Id = Guid.Parse(User.Claims.FirstOrDefault(v => v.Type == "sub").Value),
                    Phone = User.Claims.FirstOrDefault(v => v.Type == "phone").Value,
                    Name = User.Claims.FirstOrDefault(v => v.Type == "name").Value
                };
            }
        }

        protected IActionResult ActionResult(HandleResult result)
        {
            return new JsonResult(result) { StatusCode = (int)result.StatusCode };
        }
    }
}
