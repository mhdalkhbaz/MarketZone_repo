using System;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.Enums;

namespace MarketZone.Application.Features.Cash.CashTransactions.Commands.CreateCashTransaction
{
	public class CreateCashTransactionCommand : IRequest<BaseResult<long>>
	{
		public long CashRegisterId { get; set; }
		public TransactionType TransactionType { get; set; }
		public decimal Amount { get; set; }
		public DateTime? TransactionDate { get; set; }
		public ReferenceType? ReferenceType { get; set; }
		public long? ReferenceId { get; set; }
		public string Description { get; set; }
	}
}


