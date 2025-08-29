using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Services
{
	public interface IPurchaseInvoiceNumberGenerator
	{
		Task<string> GenerateAsync(CancellationToken cancellationToken = default);
	}
}


