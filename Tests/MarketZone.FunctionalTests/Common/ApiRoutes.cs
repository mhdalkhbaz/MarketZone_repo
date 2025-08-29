namespace MarketZone.FunctionalTests.Common
{
    internal static class ApiRoutes
    {
        internal static class Account
        {
            internal const string Authenticate = "/api/Account/Authenticate";
            internal const string ChangeUserName = "/api/Account/ChangeUserName";
            internal const string ChangePassword = "/api/Account/ChangePassword";
            internal const string Start = "/api/Account/Start";
        }

        internal static class Product
        {
            internal const string GetPagedListProduct = "/api/Product/GetPagedListProduct";
            internal const string GetProductById = "/api/Product/GetProductById";
            internal const string CreateProduct = "/api/Product/CreateProduct";
            internal const string UpdateProduct = "/api/Product/UpdateProduct";
            internal const string DeleteProduct = "/api/Product/DeleteProduct";
        }

        internal static class Category
        {
            internal const string GetPagedListCategory = "/api/Category/GetPagedListCategory";
            internal const string GetCategoryById = "/api/Category/GetCategoryById";
            internal const string CreateCategory = "/api/Category/CreateCategory";
            internal const string UpdateCategory = "/api/Category/UpdateCategory";
            internal const string DeleteCategory = "/api/Category/DeleteCategory";
        }

        internal static class Cash
        {
            internal const string GetPagedListExpense = "/api/Cash/GetPagedListExpense";
            internal const string GetExpenseById = "/api/Cash/GetExpenseById";
            internal const string CreateExpense = "/api/Cash/CreateExpense";
            internal const string UpdateExpense = "/api/Cash/UpdateExpense";
        }

        internal static string AddQueryString(this string url, string key, string value)
        {
            var separator = url.Contains("?") ? "&" : "?";
            return $"{url}{separator}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
        }
    }
}
