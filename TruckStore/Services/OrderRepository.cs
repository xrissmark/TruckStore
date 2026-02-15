using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class OrderRepository : IOrderRepository
    {
        public void Save(TruckOrder order)
        {
            Console.WriteLine($"[DB] Saving {order.Quantity} {order.ModelName} order to SQL Database.");
            Thread.Sleep(2000);
        }
    }
}