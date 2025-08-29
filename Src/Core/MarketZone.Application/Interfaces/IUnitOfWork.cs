using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync();
    }
}
