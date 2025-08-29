using MarketZone.Application.Features.Categories.Commands.CreateCategory;
using MarketZone.Application.Features.Categories.Commands.UpdateCategory;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Categories.DTOs;
using MarketZone.FunctionalTests.Common;
using Shouldly;

namespace MarketZone.FunctionalTests.ApiEndpoints
{
    [Collection("CategoryFunctionalTests")]
    public class CategoryFunctionalTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient client = factory.CreateClient();

        [Fact]
        public async Task GetPagedListCategory_ShouldReturnPagedResponse()
        {
            // Arrange
            var url = ApiRoutes.Category.GetPagedListCategory.AddQueryString("PageNumber", "1").AddQueryString("PageSize", "10");

            // Act
            var result = await client.GetAndDeserializeAsync<PagedResponse<CategoryDto>>(url);

            // Assert
            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnCategoryId()
        {
            // Arrange
            var url = ApiRoutes.Category.CreateCategory;
            var command = new CreateCategoryCommand
            {
                Name = RandomDataExtensionMethods.RandomString(10),
                Description = RandomDataExtensionMethods.RandomString(20)
            };
            var ghostAccount = await client.GetGhostAccount();

            // Act
            var result = await client.PostAndDeserializeAsync<BaseResult<long>>(url, command, ghostAccount.JwToken);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetCategoryById_ShouldReturnCategory()
        {
            // Arrange
            var createUrl = ApiRoutes.Category.CreateCategory;
            var createCommand = new CreateCategoryCommand
            {
                Name = RandomDataExtensionMethods.RandomString(10),
                Description = RandomDataExtensionMethods.RandomString(20)
            };
            var ghostAccount = await client.GetGhostAccount();
            var createResult = await client.PostAndDeserializeAsync<BaseResult<long>>(createUrl, createCommand, ghostAccount.JwToken);
            createResult.Success.ShouldBeTrue();
            var id = createResult.Data.ToString();

            var url = ApiRoutes.Category.GetCategoryById.AddQueryString("id", id);

            // Act
            var result = await client.GetAndDeserializeAsync<BaseResult<CategoryDto>>(url);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Id.ShouldBe(long.Parse(id));
        }

        [Fact]
        public async Task UpdateCategory_ShouldSucceed()
        {
            // Arrange
            var createUrl = ApiRoutes.Category.CreateCategory;
            var commandCreate = new CreateCategoryCommand
            {
                Name = RandomDataExtensionMethods.RandomString(10),
                Description = RandomDataExtensionMethods.RandomString(20)
            };
            var ghostAccount = await client.GetGhostAccount();
            var createResult = await client.PostAndDeserializeAsync<BaseResult<long>>(createUrl, commandCreate, ghostAccount.JwToken);
            createResult.Success.ShouldBeTrue();
            var createdId = createResult.Data;

            var url = ApiRoutes.Category.UpdateCategory;
            var command = new UpdateCategoryCommand
            {
                Id = createdId,
                Name = RandomDataExtensionMethods.RandomString(10),
                Description = RandomDataExtensionMethods.RandomString(20)
            };

            // Act
            var result = await client.PutAndDeserializeAsync<BaseResult>(url, command, ghostAccount.JwToken);

            // Assert
            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task DeleteCategory_ShouldSucceed()
        {
            // Arrange
            var createUrl = ApiRoutes.Category.CreateCategory;
            var commandCreate = new CreateCategoryCommand
            {
                Name = RandomDataExtensionMethods.RandomString(10),
                Description = RandomDataExtensionMethods.RandomString(20)
            };
            var ghostAccount = await client.GetGhostAccount();
            var createResult = await client.PostAndDeserializeAsync<BaseResult<long>>(createUrl, commandCreate, ghostAccount.JwToken);
            createResult.Success.ShouldBeTrue();
            var createdId = createResult.Data;

            var url = ApiRoutes.Category.DeleteCategory.AddQueryString("id", createdId.ToString());

            // Act
            var result = await client.DeleteAndDeserializeAsync<BaseResult>(url, ghostAccount.JwToken);

            // Assert
            result.Success.ShouldBeTrue();
        }
    }
}





