namespace MarketZone.Domain.Cash.Enums
{
	public enum PaymentType : short
	{
		// إيرادات
		SalesPayment = 0,           // دفعات فواتير المبيعات
		
		// مصروفات
		PurchasePayment = 1,        // دفعات فواتير الشراء
		RoastingPayment = 2,        // دفع أجور التحميص
		SalaryPayment = 4,          // دفع رواتب الموظفين
		GeneralExpense = 3         // مصروفات عامة
	}
}

