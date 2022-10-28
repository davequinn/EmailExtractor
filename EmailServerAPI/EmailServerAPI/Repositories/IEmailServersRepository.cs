using EmailServerAPI.Models;

namespace EmailServerAPI.Repositories
{
    public interface IEmailServersRepository
    {
        Task<IEnumerable<AutoEmail>> GetServersEmail(string email);
        Task<IEnumerable<AutoEmail>> GetAllEmailAsync();
        Task<IEnumerable<AutoEmailServers>> GetUserServers(string initials);
        Task<IEnumerable<AutoEmailServers>> GetMasterEmails();
        Task<AutoEmailServers> DeleteFile(AutoEmailServers servername);
        Task<List<AutoEmailServers>> DeleteRange(List<AutoEmailServers> servernames);
    }
}
