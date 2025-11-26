namespace MarketZone.Application.Parameters
{
    public class SalaryPaymentFilter : PaginationRequestParameter
    {
        public long? EmployeeId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public long? CashRegisterId { get; set; }
        public string Name { get; set; } // Maps to Employee name search
    }
}

