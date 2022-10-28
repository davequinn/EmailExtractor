using EmailServerAPI.Models;
using EmailServerAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmailServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoEmailController : ControllerBase
    {
        private readonly IEmailRepository autoEmail;

        public AutoEmailController(IEmailRepository autoEmail)
        {
            this.autoEmail = autoEmail;
        }

        [HttpDelete("GroupDelete")]
        public async Task<List<AutoEmail>> DeleteServer([FromBody] List<AutoEmail> emails)
        {
            return await autoEmail.DeleteRange(emails);
        }
    }
}
