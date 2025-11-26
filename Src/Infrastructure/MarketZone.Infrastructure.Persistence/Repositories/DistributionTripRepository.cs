using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MarketZone.Application.DTOs;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Parameters;
using MarketZone.Domain.Logistics.DTOs;
using MarketZone.Domain.Logistics.Entities;
using MarketZone.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MarketZone.Infrastructure.Persistence.Repositories
{
	public class DistributionTripRepository(ApplicationDbContext dbContext, IMapper mapper) : GenericRepository<DistributionTrip>(dbContext), IDistributionTripRepository
	{
		public async Task<PaginationResponseDto<DistributionTripDto>> GetPagedListAsync(DistributionTripFilter filter)
		{
			var query = dbContext.Set<DistributionTrip>()
				.Include(x => x.Details)
					.ThenInclude(d => d.Product)
				.Include(x => x.Employee)
				.Include(x => x.Car)
				.Include(x => x.Region)
				.OrderByDescending(p => p.TripDate)
				.AsQueryable();

			// Apply filters using FilterBuilder pattern
			if (filter.CarId.HasValue)
			{
				query = query.Where(p => p.CarId == filter.CarId.Value);
			}

			if (filter.RegionId.HasValue)
			{
				query = query.Where(p => p.RegionId == filter.RegionId.Value);
			}

			if (!string.IsNullOrEmpty(filter.Name))
			{
				query = query.Where(p => p.TripNumber != null && p.TripNumber.Contains(filter.Name));
			}

			// استخدام Select مباشرة لإضافة الأسماء
			var dtoQuery = query.Select(trip => new DistributionTripDto
			{
				Id = trip.Id,
				EmployeeId = trip.EmployeeId,
				EmployeeName = trip.Employee != null ? $"{trip.Employee.FirstName} {trip.Employee.LastName}" : string.Empty,
				CarId = trip.CarId,
				CarName = trip.Car != null ? trip.Car.Name : string.Empty,
				RegionId = trip.RegionId,
				RegionName = trip.Region != null ? trip.Region.Name : string.Empty,
				TripDate = trip.TripDate,
				LoadQty = trip.LoadQty,
				Notes = trip.Notes,
				TripNumber = trip.TripNumber,
				Status = trip.Status,
				Details = trip.Details.Select(d => new DistributionTripDetailDto
				{
					Id = d.Id,
					ProductId = d.ProductId,
					Qty = d.Qty,
					SoldQty = d.SoldQty,
					ReturnedQty = d.ReturnedQty,
					ExpectedPrice = d.ExpectedPrice
				}).ToList()
			});

			return await Paged(dtoQuery, filter.PageNumber, filter.PageSize);
		}

		public async Task<DistributionTrip> GetWithDetailsByIdAsync(long id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<DistributionTrip>()
				.Include(x => x.Details)
					.ThenInclude(x => x.Product)
				.Include(x => x.Employee)
				.Include(x => x.Car)
				.Include(x => x.Region)
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<bool> HasDistributionTripsAsync(long regionId, CancellationToken cancellationToken = default)
		{
			return await dbContext.Set<DistributionTrip>()
				.AnyAsync(dt => dt.RegionId == regionId, cancellationToken);
		}

		public async Task<string> GetNextTripNumberAsync(CancellationToken cancellationToken = default)
		{
			var year = DateTime.UtcNow.Year;
			var prefix = $"DT-{year}-";
			var lastForYear = await dbContext.Set<DistributionTrip>()
				.Where(t => t.TripNumber != null && t.TripNumber.StartsWith(prefix))
				.OrderByDescending(t => t.TripNumber)
				.Select(t => t.TripNumber)
				.FirstOrDefaultAsync(cancellationToken);

			int nextSeq = 1;
			if (!string.IsNullOrEmpty(lastForYear))
			{
				var parts = lastForYear.Split('-');
				if (parts.Length == 3 && int.TryParse(parts[2], out var seq))
				{
					nextSeq = seq + 1;
				}
			}

			return $"{prefix}{nextSeq.ToString("D5")}";
		}
	}
}


