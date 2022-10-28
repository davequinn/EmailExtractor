using EmailServerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailServerWeb.Data
{
    public interface IEmailServersServices
    {
        Task<IEnumerable<AutoEmailServers>> GetAll();
        Task<IEnumerable<AutoEmailServers>> GetByUser(string initials);
        Task<AutoEmailServers> DeleteServer([FromBody] AutoEmailServers server);
        Task<List<AutoEmailServers>> DeleteServer([FromBody] List<AutoEmailServers> servers);
        Task<List<AutoEmail>> DeleteEmails([FromBody] List<AutoEmail> emails_to_delete);
        Task<List<AutoEmail>> GetEmailsofServer(string servername);
        Task<List<AutoEmail>> GetEmails();
        
    }
}
