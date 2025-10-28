using MarketZone.Application.DTOs;

namespace MarketZone.Application.Helpers
{
    public static class TranslatorMessages
    {
        public static class AccountMessages
        {
            public static TranslatorMessageDto Account_NotFound_with_UserName(string userName) => new(nameof(Account_NotFound_with_UserName), [userName]);
            public static TranslatorMessageDto Username_is_already_taken(string userName) => new(nameof(Username_is_already_taken), [userName]);
            public static string Invalid_password() => nameof(Invalid_password);
        }
        public static class ProductMessages
        {
            public static TranslatorMessageDto Product_NotFound_with_id(long id)
                => new(nameof(Product_NotFound_with_id), [id.ToString()]);
        }

		public static class CategoryMessages
		{
			public static TranslatorMessageDto Category_NotFound_with_id(long id)
				=> new(nameof(Category_NotFound_with_id), [id.ToString()]);
		}

		public static class CustomerMessages
		{
			public static TranslatorMessageDto Customer_NotFound_with_id(long id)
				=> new(nameof(Customer_NotFound_with_id), [id.ToString()]);
		}

		public static class SupplierMessages
		{
			public static TranslatorMessageDto Supplier_NotFound_with_id(long id)
				=> new(nameof(Supplier_NotFound_with_id), [id.ToString()]);
		}

		public static class EmployeeMessages
		{
			public static TranslatorMessageDto Employee_NotFound_with_id(long id)
				=> new(nameof(Employee_NotFound_with_id), [id.ToString()]);
		}

		public static class PurchaseInvoiceMessages
		{
			public static TranslatorMessageDto PurchaseInvoice_NotFound_with_id(long id)
				=> new(nameof(PurchaseInvoice_NotFound_with_id), [id.ToString()]);
		}

		public static class SalesInvoiceMessages
		{
			public static TranslatorMessageDto SalesInvoice_NotFound_with_id(long id)
				=> new(nameof(SalesInvoice_NotFound_with_id), [id.ToString()]);
		}

		public static class ExpenseMessages
		{
			public static TranslatorMessageDto Expense_NotFound_with_id(long id)
				=> new(nameof(Expense_NotFound_with_id), [id.ToString()]);
		}

        public static class CarMessages
        {
            public static TranslatorMessageDto Car_NotFound_with_id(long id)
                => new(nameof(Car_NotFound_with_id), [id.ToString()]);
        }

        public static class RegionMessages
        {
            public static TranslatorMessageDto Region_NotFound_with_id(long id)
                => new(nameof(Region_NotFound_with_id), [id.ToString()]);
        }

        public static class CashRegisterMessages
        {
            public static TranslatorMessageDto CashRegister_NotFound_with_id(long id)
                => new(nameof(CashRegister_NotFound_with_id), [id.ToString()]);
        }
    }
}
