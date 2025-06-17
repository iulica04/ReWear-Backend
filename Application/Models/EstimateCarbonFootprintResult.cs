namespace Application.Models
{
    public class EstimateCarbonFootprintResult
    {
        public decimal TotalCarbonFootprint { get; set; }
        public int TotalNumberOfItems { get; set; }
        public int CountedItems { get; set; }
    }
}
