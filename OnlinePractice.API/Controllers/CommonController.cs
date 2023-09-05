using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Services;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi =true)]
    public class CommonController : ControllerBase
    {
        public CommonController() { }

        [HttpPost]
        [Route("decrypturl")]
        public IActionResult DecryptURL(string url)
        {
           var result  = UrlEncryptor.DecryptUrl(url);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }

        }

    }
}
