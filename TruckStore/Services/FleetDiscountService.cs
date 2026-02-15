using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class FleetDiscountService : IDiscountService
    {
        public decimal CalculateDiscount(TruckOrder order)
        {
            Console.WriteLine($"[Service] Calculating applicable discount...");
            Thread.Sleep(1000);
            return order.Quantity > 5 ? order.BasePrice * order.Quantity * 0.10m : 0; ;
        }
    }
}
