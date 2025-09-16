using AutoMapper;
using MarketZone.Application.Interfaces;
using MarketZone.Application.Interfaces.Repositories;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Products.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MarketZone.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateProductCommand, BaseResult<long>>
    {
        public async Task<BaseResult<long>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = mapper.Map<Product>(request);
            try
            {
                await productRepository.AddAsync(product);
                await unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
            return product.Id;
        }
    }
}
