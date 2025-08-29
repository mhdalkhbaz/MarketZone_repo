using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Services
{
    public interface IRoastingInvoiceNumberGenerator
    {
        Task<string> GenerateAsync();
    }
}
