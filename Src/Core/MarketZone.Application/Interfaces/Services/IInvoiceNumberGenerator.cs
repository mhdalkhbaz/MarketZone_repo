using System.Threading;
using System.Threading.Tasks;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Interfaces.Services
{
	public interface IInvoiceNumberGenerator
	{
		Task<string> GenerateAsync(InvoiceType type, CancellationToken cancellationToken = default);
	}
}




