using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Interfaces.Services
{
	public interface ISalesInvoiceNumberGenerator
	{
		Task<string> GenerateAsync(CancellationToken cancellationToken = default);
	}
}


