using EmailServerAPI.Models;

namespace EmailServerAPI.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        public readonly pPrismMasterContext appDBContext;
        public EmailRepository(pPrismMasterContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
        public async Task<List<AutoEmail>> DeleteRange(List<AutoEmail> emails)
        {
            try
            {
                foreach (AutoEmail server in emails)
                {
                    appDBContext.AutoEmail.Remove(server);
                }
                await appDBContext.SaveChangesAsync();
                return emails;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
