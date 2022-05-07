namespace MarketZone.Domain.Cash.DTOs
{
    public class SelectList
    {
        public SelectList()
        {
            
        }
        public SelectList(long value, string label)
        {
            Value = value;
            Label = label;
        }

        public long Value { get; set; }
        public string Label { get; set; }
    }
}