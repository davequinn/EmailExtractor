using EmailServerAPI.Models;
using EmailServerAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmailServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoEmailServersController : ControllerBase
    {
        private readonly IEmailServersRepository autoEmailServers;

        public AutoEmailServersController(IEmailServersRepository autoEmailServers)
        {
            this.autoEmailServers = autoEmailServers;
        }

        [HttpGet]
        public async Task<IEnumerable<AutoEmailServers>> GetAll()
        {
            return await autoEmailServers.GetMasterEmails();
        }

        [HttpGet("{initials}")]
        public async Task<IEnumerable<AutoEmailServers>> GetByUser(string initials)
        {
            return await autoEmailServers.GetUserServers(initials);
        }

        [HttpGet("GetMail/{email}")]
        public async Task<IEnumerable<AutoEmail>> GetMailtoServer(string email)
        {
            return await autoEmailServers.GetServersEmail(email);
        }

        [HttpGet("GetAllEmails")]
        public async Task<IEnumerable<AutoEmail>> GetAllEmails()
        {
            return (await autoEmailServers.GetAllEmailAsync()).ToList()
                .OrderBy(x => x.Email)
                .ThenByDescending(x => x.Name)
                .ThenByDescending(x => x.User)
                .GroupBy(e => e.Email.ToUpper())
                .Select(e => e.First())
                .ToList();
        }

        [HttpDelete]
        public async Task<AutoEmailServers> DeleteServer([FromBody] AutoEmailServers server)
        {
            return await autoEmailServers.DeleteFile(server);
        }

        [HttpDelete("GroupDelete")]
        public async Task<List<AutoEmailServers>> DeleteServer([FromBody] List<AutoEmailServers> servers)
        {
            return await autoEmailServers.DeleteRange(servers);
        }

    }
}
