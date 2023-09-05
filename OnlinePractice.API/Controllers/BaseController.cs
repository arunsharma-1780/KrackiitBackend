using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public Guid UserId => Guid.Parse(((ClaimsIdentity)User.Identity).Claims.SingleOrDefault(x => x.Type.ToLower() == "userid")?.Value);
    }
}
