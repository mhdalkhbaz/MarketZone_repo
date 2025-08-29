using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;

namespace MarketZone.Application.Features.Employees.Commands.DeleteEmployee
{
	public class DeleteEmployeeCommand : IRequest<BaseResult>
	{
		public long Id { get; set; }
	}
}



