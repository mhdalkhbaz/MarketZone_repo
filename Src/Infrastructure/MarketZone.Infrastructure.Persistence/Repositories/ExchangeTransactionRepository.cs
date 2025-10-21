using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Domain.Cash.Entities;
using MarketZone.Domain.Cash.Enums;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class ExchangeTransactionRepository : GenericRepository<ExchangeTransaction>, IExchangeTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ExchangeTransactionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ExchangeTransaction>> GetByCashRegisterAsync(long cashRegisterId, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.CashRegisterId == cashRegisterId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExchangeTransaction>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExchangeTransaction>> GetByDirectionAsync(ExchangeDirection direction, CancellationToken cancellationToken)
        {
            return await _context.Set<ExchangeTransaction>()
                .Where(t => t.Direction == direction)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }
    }
}
