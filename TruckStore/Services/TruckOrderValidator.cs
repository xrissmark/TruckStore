using TruckStore.Models;
using TruckStore.Services.Interfaces;

namespace TruckStore.Services
{
    public class TruckOrderValidator : IOrderValidator
    {
        public bool Validate(TruckOrder order)
        {
            Console.WriteLine("[Validation] Begining order validation.");
            Thread.Sleep(1000);

            if (order == null)
            {
                Console.WriteLine("[Validation Error] Order cannot be null.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(order.ModelName))
            {
                Console.WriteLine("[Validation Error] Truck model name is required.");
                return false;
            }

            if (order.Quantity <= 0)
            {
                Console.WriteLine("[Validation Error] Quantity must be at least 1 truck.");
                return false;
            }

            if (order.BasePrice <= 0)
            {
                Console.WriteLine("[Validation Error] Base price must be greater than zero.");
                return false;
            }

            Console.WriteLine($"[Validation] Order for {order.Quantity} {order.ModelName} verified.");
            return true;
        }
    }
}
