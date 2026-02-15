namespace TruckStore.Models
{
    public class TruckOrder
    {
        public string ModelName { get; set; }
        public int Quantity { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
