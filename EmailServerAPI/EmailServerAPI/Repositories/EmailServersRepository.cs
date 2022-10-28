using EmailServerAPI.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmailServerAPI.Repositories
{
    public class EmailServersRepository : IEmailServersRepository
    {
        public readonly pPrismMasterContext appDBContext;
        public EmailServersRepository(pPrismMasterContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        public async Task<AutoEmailServers> DeleteFile(AutoEmailServers servername)
        {
            var mydeletable = servername;
            if (mydeletable != null)
            {
                appDBContext.AutoEmailServers.Remove(mydeletable);
                await appDBContext.SaveChangesAsync();
                return mydeletable;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<AutoEmail>> GetEmails()
        {
            return await appDBContext.AutoEmail.ToListAsync();
        }
        public async Task<List<AutoEmailServers>> DeleteRange(List<AutoEmailServers> servernames)
        {
            try
            {
                foreach (AutoEmailServers server in servernames)
                {
                    appDBContext.AutoEmailServers.Remove(server);
                }
                await appDBContext.SaveChangesAsync();
                return servernames;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<IEnumerable<AutoEmailServers>> GetMasterEmails()
        {
            return await appDBContext.AutoEmailServers.ToListAsync();
        }

        public async Task<IEnumerable<AutoEmail>> GetServersEmail(string email)
        {
            var kk = (await appDBContext.AutoEmail.ToListAsync()).Where(e => e.Email.Split("@").Length == 2 && e.Email.Split("@")[1].ToUpper() == email.ToUpper()).ToList();
            return (IEnumerable<AutoEmail>) kk;
        }

        public async Task<IEnumerable<AutoEmailServers>> GetUserServers(string initials)
        {
            return await appDBContext.AutoEmailServers.Where(e => e.EmployeesOutlook == initials).ToListAsync();
        }

        public async Task<IEnumerable<AutoEmail>> GetAllEmailAsync()
        {
            return await appDBContext.AutoEmail.ToListAsync();
        }
    }
}
