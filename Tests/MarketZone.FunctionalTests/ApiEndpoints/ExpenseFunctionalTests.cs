using MarketZone.Application.Features.Cash.Expenses.Commands.CreateExpense;
using MarketZone.Application.Wrappers;
using MarketZone.Domain.Cash.DTOs;
using MarketZone.FunctionalTests.Common;
using Shouldly;

namespace MarketZone.FunctionalTests.ApiEndpoints
{
    [Collection("ExpenseFunctionalTests")]
    public class ExpenseFunctionalTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient client = factory.CreateClient();

        [Fact]
        public async Task GetPagedListExpense_ShouldReturnPagedResponse()
        {
            // Arrange
            var url = ApiRoutes.Cash.GetPagedListExpense.AddQueryString("PageNumber", "1").AddQueryString("PageSize", "10");

            // Act
            var result = await client.GetAndDeserializeAsync<PagedResponse<ExpenseDto>>(url);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task CreateExpense_ShouldReturnExpenseId()
        {
            // Arrange
            var url = ApiRoutes.Cash.CreateExpense;
            var command = new CreateExpenseCommand
            {
                Amount = 1000.00m,
                ExpenseDate = DateTime.Now,
                Description = "Test Expense"
            };
            var ghostAccount = await client.GetGhostAccount();

            // Act
            var result = await client.PostAndDeserializeAsync<BaseResult<long>>(url, command, ghostAccount.JwToken);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task GetExpenseById_ShouldReturnExpense()
        {
            // Arrange - First create an expense
            var createUrl = ApiRoutes.Cash.CreateExpense;
            var createCommand = new CreateExpenseCommand
            {
                Amount = 1500.00m,
                ExpenseDate = DateTime.Now,
                Description = "Test Expense for GetById"
            };
            var ghostAccount = await client.GetGhostAccount();

            var createResult = await client.PostAndDeserializeAsync<BaseResult<long>>(createUrl, createCommand, ghostAccount.JwToken);
            createResult.Success.ShouldBeTrue();
            var expenseId = createResult.Data;

            // Act - Get the expense by ID
            var getUrl = ApiRoutes.Cash.GetExpenseById.AddQueryString("Id", expenseId.ToString());
            var result = await client.GetAndDeserializeAsync<BaseResult<ExpenseDto>>(getUrl);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Id.ShouldBe(expenseId);
            result.Data.Amount.ShouldBe(1500.00m);
            result.Data.Description.ShouldBe("Test Expense for GetById");
        }
    }
}
