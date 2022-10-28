using EmailServerAPI.Models;

namespace EmailServerAPI.Repositories
{
    public interface IEmailRepository
    {
        Task<List<AutoEmail>> DeleteRange(List<AutoEmail> emails);

    }
}
