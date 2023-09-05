using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlinePractice.API.Models.AuthDB;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService mailService;
        public EmailController(IEmailService mailService)
        {
            this.mailService = mailService;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromForm] EmailData request)
        {
            try
            {
                await mailService.SendEmail(request);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
