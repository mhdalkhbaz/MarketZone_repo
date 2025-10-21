namespace MarketZone.Domain.Cash.Enums
{
	public enum PaymentType : short
	{
		// Income (إيرادات - مرتبطة بالفواتير)
		SalesPayment = 0,           // دفعات فواتير المبيعات
		
		// Expense (مصروفات - مرتبطة بالفواتير)
		PurchasePayment = 1,        // دفعات فواتير الشراء
		RoastingPayment = 2,         // دفع أجور التحميص
		
		// Expense (مصروفات - غير مرتبطة بالفواتير)
		GeneralExpense = 3,         // مصروفات عامة
		OfficeExpense = 4,          // مصروفات مكتبية
		TransportationExpense = 5,   // مصروفات نقل
		MaintenanceExpense = 6,     // مصروفات صيانة
		OtherExpense = 7            // مصروفات أخرى
	}
}

