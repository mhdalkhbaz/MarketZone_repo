using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Entities;

namespace MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense
{
	public class CreateExpenseCommandHandler(IExpenseRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateExpenseCommand, BaseResult<long>>
	{
		public async Task<BaseResult<long>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
		{
			var entity = mapper.Map<Expense>(request);
			await repository.AddAsync(entity);
			await unitOfWork.SaveChangesAsync();
			return entity.Id;
		}
	}
}


