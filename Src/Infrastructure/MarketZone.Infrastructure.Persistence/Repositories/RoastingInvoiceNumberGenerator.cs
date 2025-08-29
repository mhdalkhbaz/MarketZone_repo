using System;
using System.Linq;
using System.Threading.Tasks;
using MarketZone.Application.Interfaces.Services;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
    public class RoastingInvoiceNumberGenerator : IRoastingInvoiceNumberGenerator
    {
        private readonly ApplicationDbContext _context;

        public RoastingInvoiceNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateAsync()
        {
            var currentYear = DateTime.Now.Year;
            var yearPrefix = $"RM-{currentYear}-";

            // Get the last invoice number for this year
            var lastInvoice = await _context.RoastingInvoices
                .Where(x => x.InvoiceNumber.StartsWith(yearPrefix))
                .OrderByDescending(x => x.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastInvoice != null)
            {
                var lastNumberStr = lastInvoice.InvoiceNumber.Replace(yearPrefix, "");
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{yearPrefix}{nextNumber:D5}";
        }
    }
}
